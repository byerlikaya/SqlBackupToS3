namespace SqlBackupToS3.Options;

public class BackupOption
{
    private int _dailyRepeat;

    public string ConnectionString { get; set; }

    public string BackupFolderPath { get; set; }

    public int DailyRepeat
    {
        get =>
            _dailyRepeat is default(int)
            ? 1
            : _dailyRepeat;

        set =>
            _dailyRepeat = value switch
            {
                <= 0 => 1,
                > 24 => 24,
                _ => value
            };
    }

    public bool DeleteAfterZip { get; set; }

    public bool DebugMode { get; set; }

    public AmazonCredentialOptions AmazonCredentialOptions { get; set; }

    public AmazonS3Options AmazonS3Options { get; set; }

}