namespace SqlBackupToS3.Utilities;

internal class ZipArchiveTool(BackupOption backupOption)
{
    public void MakeZipFile(string filePath)
    {
        Archive(filePath);
        if (backupOption.DeleteAfterZip)
            File.Delete(filePath);
    }

    private static void Archive(string filePath)
    {
        var zipFilePath = filePath.CreateFilePathForZip();
        using var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);
        archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
    }
}
