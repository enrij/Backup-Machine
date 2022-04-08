namespace BackupMachine.PoC;

public static class TaskSettings
{
    static TaskSettings()
    {
        Sources = new List<string> { @"C:\Temp\Sources\SmallSource", /*@"C:\Temp\Sources\LargeSource", */@"C:\Temp\Sources\InvalidSource" };
        Destinations = new List<string> { @"C:\Temp\Backups\Destination1", @"C:\Temp\Backups\Destination2" };
    }

    public static IEnumerable<string> Sources { get; }
    public static IEnumerable<string> Destinations { get; }
}
