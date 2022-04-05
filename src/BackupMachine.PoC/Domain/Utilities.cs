using BackupMachine.PoC.Domain.Entities;

namespace BackupMachine.PoC.Domain;

public static class Utilities
{
    public static string ComposeBackupArchiveName(Backup backup)
    {
        return $"BackupMachine-{backup.Timestamp:yyyy MM dd HH mm ss}.zip";
    }

    public static string ComposeTemporaryFolderPath(Backup backup)
    {
        return Path.Combine(Path.GetTempPath(), "BackupMachine", backup.Id.ToString());
    }
}
