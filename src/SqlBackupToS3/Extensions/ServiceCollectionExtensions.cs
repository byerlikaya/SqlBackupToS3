namespace SqlBackupToS3.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlBackup(
        this IServiceCollection services,
        Action<BackupOption> options)
    {
        ArgumentNullControl(services, options);

        DefineServices(services, options);

        return services;
    }

    private static void DefineServices(
        IServiceCollection services,
        Action<BackupOption> options)
    {
        //Details => https://github.com/byerlikaya/AmazonWebServices
        services.AddAmazonWebServices();

        services.AddScoped<SqlBackup>();

        services.AddSingleton(_ =>
        {
            var optionInstance = new BackupOption();
            options(optionInstance);
            return optionInstance;
        });

        services.AddHostedService<BackupBackgroundService>();
    }

    private static void ArgumentNullControl(
        IServiceCollection services,
        Action<BackupOption> options)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (options is null) throw new ArgumentNullException(nameof(options));
    }
}