using BackupMachine.Core.Enums;

namespace BackupMachine.Core.Entities;

public class BackupFile
{
    public Guid Id { get; set; }
    public Guid BackupId { get; set; }
    public Guid BackupFolderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public FileStatus Status { get; set; }
    public long Length { get; set; }
    public DateTime Modified { get; set; }
    public DateTime Created { get; set; }

    public virtual Backup Backup { get; set; } = default!;
    public virtual BackupFolder BackupFolder { get; set; } = default!;
}
