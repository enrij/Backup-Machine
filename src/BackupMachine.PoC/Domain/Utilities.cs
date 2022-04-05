using BackupMachine.PoC.Domain.Entities;

namespace BackupMachine.PoC.Domain;

public static class Utilities
{
    public static string ComposeBackupArchiveName(Backup backup)
    {
        return $"BackupMachine-{backup.Timestamp:yyyy MM dd HH mm ss}.zip";
    }
}
