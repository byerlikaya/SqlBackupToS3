# Sql Backup to S3
#### Backup the database, zip it and upload it to s3.

![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/byerlikaya/SqlBackupToS3/dotnet.yml)
[![SqlBackupToS3 Nuget](https://img.shields.io/nuget/v/SqlBackupToS3)](https://www.nuget.org/packages/SqlBackupToS3)
[![SqlBackupToS3 Nuget](https://img.shields.io/nuget/dt/SqlBackupToS3)](https://www.nuget.org/packages/SqlBackupToS3)

Setup in just 3 steps.

1. Install **SqlBackupToS3** NuGet package from [here](https://www.nuget.org/packages/SqlBackupToS3/).

````
PM> Install-Package SqlBackupToS3
````

2. Add services.AddSqlBackup();

```csharp
builder.Services.AddSqlBackup(x =>
{
    x.ConnectionString = "YOUR_CONNECTION_STRING";
    x.FolderPath = "BACKUP_FOLDER_PATH";
    x.DailyRepeat = 2;
});
```
3. Add the necessary information to the `appsettings.json` file.

```json
 "AmazonCredentialOptions": {
    "AccessKey": "",
    "SecretKey": ""
  },

  "AmazonS3Options": {
    "BucketName": "",
    "Region": "eu-central-1"
  }
```
Give a star ⭐, fork and stay tuned.
