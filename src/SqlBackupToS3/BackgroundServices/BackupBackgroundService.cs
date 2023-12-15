namespace SqlBackupToS3.BackgroundServices;

public class BackupBackgroundService(
    IServiceProvider serviceProvider,
    SqlBackupOption options,
    ILogger<BackupBackgroundService> logger) : BackgroundService
{
    private DateTime _nextRunTime = DateTime.UtcNow;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Backup Background Service is starting.");

        await RunBackup();

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Backup Background Service is working.");

                await Task.Delay(GetDelayTime(), cancellationToken);

                await RunBackup();

                _nextRunTime = GetNextDate();
            }
            catch (Exception exception)
            {
                logger.LogError($"Backup Background Service encountered an error. Error Message : {exception.Message}");
            }
        }
    }

    private async Task RunBackup()
    {
        using var scope = serviceProvider.CreateScope();
        var sqlTool = scope.ServiceProvider.GetRequiredService<SqlTool>();
        var zipArchiveTool = scope.ServiceProvider.GetRequiredService<ZipArchiveTool>();
        var amazonS3Service = scope.ServiceProvider.GetRequiredService<IAmazonS3Service>();

        var fileName = sqlTool.BackupFile();
        var zipFilePath = zipArchiveTool.MakeZipFileAndDelete(fileName);

        await amazonS3Service.UploadAsync(new UploadRequest
        {
            FilePath = zipFilePath
        });
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