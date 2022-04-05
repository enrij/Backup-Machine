namespace BackupMachine.PoC.Domain.Entities;

public class BackupFile
{
    public Guid Id { get; set; }
    public Guid BackupId { get; set; }
    public Guid BackupFolderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;

    public Backup Backup { get; set; } = default!;
    public BackupFolder BackupFolder { get; set; } = default!;
}
