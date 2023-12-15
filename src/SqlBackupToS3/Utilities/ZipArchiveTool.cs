namespace SqlBackupToS3.Utilities;

internal class ZipArchiveTool
{
    public string MakeZipFile(string filePath) => Archive(filePath);

    public string MakeZipFileAndDelete(string filePath)
    {
        var zipFilePath = Archive(filePath);
        File.Delete(filePath);
        return zipFilePath;
    }

    private string Archive(string filePath)
    {
        var zipFilePath = $"{filePath}.zip";
        using var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);
        archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
        return zipFilePath;
    }
}
