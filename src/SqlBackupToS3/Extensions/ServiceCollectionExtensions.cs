namespace SqlBackupToS3.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlBackup(
        this IServiceCollection services,
        Action<SqlBackupOption> options)
    {
        ArgumentNullControl(services, options);

        SingletonServices(services, options);

        //https://github.com/byerlikaya/AmazonWebServices
        services.AddAmazonWebServices();

        return services;
    }

    private static void SingletonServices(
        IServiceCollection services,
        Action<SqlBackupOption> options)
    {
        services.AddSingleton<SqlTool>();
        services.AddSingleton<ZipArchiveTool>();

        services.AddSingleton(_ =>
        {
            var optionInstance = new SqlBackupOption();
            options(optionInstance);
            return optionInstance;
        });

        services.AddHostedService<BackupBackgroundService>();
    }

    private static void ArgumentNullControl(
        IServiceCollection services,
        Action<SqlBackupOption> options)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (options is null) throw new ArgumentNullException(nameof(options));
    }
}