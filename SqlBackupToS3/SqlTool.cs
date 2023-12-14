namespace SqlBackupToS3;

public static class SqlTool
{
    private const string FolderPath = @"D:\Backup";

    public static string BackupFile()
    {
        var backupName = CreateBackupName();

        var filePath = CreateFilePath(backupName);

        var server = CreateServer();

        var backup = CreateBackup(server, backupName, filePath);

        backup.SqlBackup(server);

        return filePath;
    }

    private static Server CreateServer() =>
        new(new ServerConnection { ConnectionString = GetSqlConnectionString().ConnectionString });

    private static Backup CreateBackup(IAlienRoot server, string backupName,
        string filePath)
    {
        var sqlConnectionString = GetSqlConnectionString();

        var backup = new Backup
        {
            Action = BackupActionType.Database,
            Database = sqlConnectionString.InitialCatalog,
            BackupSetName = backupName,
            BackupSetDescription = $"Backup for database {sqlConnectionString.InitialCatalog} from server {server.Name}",
            Initialize = true,
            Incremental = false
        };

        backup.Devices.AddDevice(filePath, DeviceType.File);

        return backup;
    }

    private static string CreateFilePath(string backupName) => Path.Combine(FolderPath, $"{backupName}.bak");

    private static string CreateBackupName() => $"{GetSqlConnectionString().InitialCatalog}-Backup-{DateTime.Now:yyyy-MM-ddTHHmmss}";

    private static SqlConnectionStringBuilder GetSqlConnectionString() => new()
    {
        ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DataAuditing;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;"
    };
}