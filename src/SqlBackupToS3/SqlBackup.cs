namespace SqlBackupToS3;

internal class SqlBackup(IAmazonS3Service amazonS3Service, BackupOption backupOption)
{
    public void BackupAndZipUploadToS3V1()
    {
        var sqlConnectionString = GetSqlConnectionString();

        var backupName = sqlConnectionString.InitialCatalog.CreateBackupName();

        var filePath = backupName.CreateFilePathForBackup(backupOption.BackupFolderPath);

        var backupQuery = $"BACKUP DATABASE {sqlConnectionString.InitialCatalog} TO DISK='{filePath}'";


        using SqlConnection connection = new(sqlConnectionString.ConnectionString);

        SqlCommand command = new(backupQuery, connection);

        connection.Open();

        command.ExecuteNonQuery();

        connection.Close();

        var zipArchiveTool = new ZipArchiveTool(backupOption);
        zipArchiveTool.MakeZipFile(filePath);

        var zipFilePath = filePath.CreateFilePathForZip();

        amazonS3Service.UploadExternalAwsAsync(new UploadExternalAwsRequest
        {
            FilePath = zipFilePath,
            AmazonS3Options = backupOption.AmazonS3Options,
            AmazonCredentialOptions = backupOption.AmazonCredentialOptions
        });
    }

    public void BackupAndZipUploadToS3()
    {
        var server = CreateServer();

        var backup = CreateBackup(server);

        backup.SqlBackupAsync(server);
    }

    private Server CreateServer() =>
        new(new ServerConnection { ConnectionString = GetSqlConnectionString().ConnectionString });

    private Backup CreateBackup(
        IAlienRoot server)
    {
        var sqlConnectionString = GetSqlConnectionString();

        var backupName = sqlConnectionString.InitialCatalog.CreateBackupName();

        var filePath = backupName.CreateFilePathForBackup(backupOption.BackupFolderPath);

        var backup = new Backup
        {
            Action = BackupActionType.Database,
            Database = sqlConnectionString.InitialCatalog,
            BackupSetName = backupName,
            BackupSetDescription = $"Backup for database {sqlConnectionString.InitialCatalog} from server {server.Name}",
            Initialize = true,
            Incremental = false
        };

        backup.Complete += Backup_Complete;

        backup.Devices.AddDevice(filePath, DeviceType.File);

        return backup;
    }

    private void Backup_Complete(object sender, ServerMessageEventArgs e)
    {
        if (sender is not Backup backup)
            return;

        var backupFilePath = backup.BackupSetName.CreateFilePathForBackup(backupOption.BackupFolderPath);

        var zipArchiveTool = new ZipArchiveTool(backupOption);
        zipArchiveTool.MakeZipFile(backupFilePath);

        var zipFilePath = backupFilePath.CreateFilePathForZip();

        amazonS3Service.UploadExternalAwsAsync(new UploadExternalAwsRequest
        {
            FilePath = zipFilePath,
            AmazonS3Options = backupOption.AmazonS3Options,
            AmazonCredentialOptions = backupOption.AmazonCredentialOptions
        });
    }

    private SqlConnectionStringBuilder GetSqlConnectionString() =>
        new() { ConnectionString = backupOption.ConnectionString };
}