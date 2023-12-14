// See https://aka.ms/new-console-template for more information

using SqlBackupToS3;

Console.WriteLine("Hello, World!");

var filePath = SqlTool.BackupFile();

ZipTool.MakeZipFile(filePath);

Console.WriteLine(filePath);