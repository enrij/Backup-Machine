namespace BackupMachine.PoC.Domain.Entities;

public class Backup
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public DateTime Timestamp { get; set; }

    public Job Job { get; set; } = default!;
}
