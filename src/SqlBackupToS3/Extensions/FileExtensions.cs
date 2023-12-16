namespace SqlBackupToS3.Extensions;

internal static class FileExtensions
{
    public static string CreateBackupName(this string initialCatalog) =>
        $"{initialCatalog}-Backup-{DateTime.Now:yyyy-MM-ddTHHmmss}";

    public static string CreateFilePathForBackup(this string backupName, string folderPath)
    {
        DirectoryControl(folderPath);
        return Path.Combine(folderPath, $"{backupName}.bak");
    }

    public static string CreateFilePathForZip(this string filePath) =>
        $"{filePath.Replace(Path.GetExtension(filePath), string.Empty)}.zip";

    private static void DirectoryControl(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
    }

}