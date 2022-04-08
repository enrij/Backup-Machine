namespace BackupMachine.PoC.Domain.Entities;

public class BackupFolder
{
    public Guid Id { get; set; }
    public Guid BackupId { get; set; }
    public Guid? ParentFolderId { get; set; }
    public string RelativePath { get; set; } = string.Empty;

    public virtual Backup Backup { get; set; } = default!;
    public virtual BackupFolder? ParentFolder { get; set; }
    public virtual List<BackupFile> Files { get; set; } = new();
    public virtual List<BackupFolder> Subfolders { get; set; } = new();
}
