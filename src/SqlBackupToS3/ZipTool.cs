namespace SqlBackupToS3;

public static class ZipTool
{
    public static void MakeZipFile(string filePath) => Archive(filePath);

    public static void MakeZipFileAndDelete(string filePath)
    {
        Archive(filePath);
        File.Delete(filePath);
    }

    private static void Archive(string filePath)
    {
        var zipFilePath = $"{filePath}.zip";
        using var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);
        archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
    }
}
