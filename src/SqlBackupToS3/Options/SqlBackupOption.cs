namespace SqlBackupToS3.Options;

public class SqlBackupOption
{
    private int _dailyRepeat;

    public string FolderPath { get; set; }

    public string ConnectionString { get; set; }

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
}