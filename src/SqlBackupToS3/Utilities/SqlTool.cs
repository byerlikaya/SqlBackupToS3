namespace SqlBackupToS3.Utilities;

internal class SqlTool(SqlBackupOption sqlBackupOption)
{
    public string BackupFile()
    {
        var backupName = CreateBackupName();

        var filePath = CreateFilePath(backupName);

        var server = CreateServer();

        var backup = CreateBackup(server, backupName, filePath);

        backup.SqlBackup(server);

        return filePath;
    }

    private Server CreateServer() =>
        new(new ServerConnection { ConnectionString = GetSqlConnectionString().ConnectionString });

    private Backup CreateBackup(
        IAlienRoot server,
        string backupName,
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

    private string CreateFilePath(string backupName)
    {
        if (!Directory.Exists(sqlBackupOption.FolderPath))
            Directory.CreateDirectory(sqlBackupOption.FolderPath);
        return Path.Combine(sqlBackupOption.FolderPath, $"{backupName}.bak");
    }

    private string CreateBackupName() =>
        $"{GetSqlConnectionString().InitialCatalog}-Backup-{DateTime.Now:yyyy-MM-ddTHHmmss}";

    private SqlConnectionStringBuilder GetSqlConnectionString() =>
        new() { ConnectionString = sqlBackupOption.ConnectionString };
}