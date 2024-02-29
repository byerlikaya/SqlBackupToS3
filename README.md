# Sql Backup to Amazon S3
#### Backup the database, zip it and upload it to s3.

![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/byerlikaya/SqlBackupToS3/dotnet.yml)
[![SqlBackupToS3 Nuget](https://img.shields.io/nuget/v/SqlBackupToS3)](https://www.nuget.org/packages/SqlBackupToS3)
[![SqlBackupToS3 Nuget](https://img.shields.io/nuget/dt/SqlBackupToS3)](https://www.nuget.org/packages/SqlBackupToS3)

Setup in just 2 steps.

1. Install **SqlBackupToS3** NuGet package from [here](https://www.nuget.org/packages/SqlBackupToS3/).

````
PM> Install-Package SqlBackupToS3
````

2. Add services.AddSqlBackup();

```csharp
builder.Services.AddSqlBackup(x =>
{
    x.ConnectionString = "YOUR_CONNECTION_STRING";
    x.BackupFolderPath = "BACKUP_FOLDER_PATH";
    x.DailyRepeat = 2;
    x.DeleteAfterZip = true;
    x.DebugMode = false;
    x.BackupOnStartup = false;
    x.AmazonCredentialOptions = new AmazonCredentialOptions
    {
        AccessKey = "YOUR_ACCESS_KEY",
        SecretKey = "YOUR_SECRET_KEY"
    };
    x.AmazonS3Options = new AmazonS3Options
    {
        BucketName = "YOUR_BUCKET_NAME",
        Region = "eu-central-1"
    };
});
```

`DebugMode` : Database backups on the remote server will give an error because the specified backup file cannot be found on your local computer. If you are not working on the local database, you should set it to "true". In short, it should be set to "false" if it is an application running on the same server as the database being backed up.

Give a star ⭐, fork and stay tuned.
