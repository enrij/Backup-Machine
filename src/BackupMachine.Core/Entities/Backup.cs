namespace BackupMachine.Core.Entities;

public class Backup
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid? PreviousBackupId { get; set; }

    public Job Job { get; set; } = default!;
    public Backup? PreviousBackup { get; set; }
}
