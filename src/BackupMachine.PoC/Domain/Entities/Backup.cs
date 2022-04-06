namespace BackupMachine.PoC.Domain.Entities;

public class Backup
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid? PreviousBackupId { get; set; }

    public virtual Job Job { get; set; } = default!;
    public virtual Backup? PreviousBackup { get; set; }
}
