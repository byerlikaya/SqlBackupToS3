namespace SqlBackupToS3.BackgroundServices;

public class BackupBackgroundService(
    IServiceProvider serviceProvider,
    BackupOption options,
    ILogger<BackupBackgroundService> logger) : BackgroundService
{
    private DateTime _nextRunTime = DateTime.UtcNow;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Backup Background Service is starting.");

        RunBackup();

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Backup Background Service is working.");

                await Task.Delay(GetDelayTime(), cancellationToken);

                RunBackup();

                _nextRunTime = GetNextDate();
            }
            catch (Exception exception)
            {
                logger.LogError($"Backup Background Service encountered an error. Error Message : {exception.Message}");
            }
        }
    }

    private void RunBackup()
    {
        if (options.DebugMode)
            return;

        if (!options.BackupOnStartup)
            return;

        using var scope = serviceProvider.CreateScope();
        var sqlBackup = scope.ServiceProvider.GetRequiredService<SqlBackup>();
        sqlBackup.BackupAndZipUploadToS3V1();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Backup Background Service has been stopped.");
        return base.StopAsync(cancellationToken);
    }

    private int GetDelayTime()
    {
        var delayTime = DelayTime();

        if (delayTime > 0)
            return (int)delayTime;

        _nextRunTime = GetNextDate();

        return (int)DelayTime();
    }

    private DateTime GetNextDate()
    {
        var repeat = 24 / options.DailyRepeat;
        return DateTime.UtcNow.AddHours(repeat);
    }

    private double DelayTime() => (_nextRunTime - DateTime.UtcNow).TotalMilliseconds;
}