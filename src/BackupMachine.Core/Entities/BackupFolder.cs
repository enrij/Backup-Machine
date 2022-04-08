namespace BackupMachine.Core.Entities;

public class BackupFolder
{
    public Guid Id { get; set; }
    public Guid BackupId { get; set; }
    public Guid? ParentFolderId { get; set; }
    public string RelativePath { get; set; } = string.Empty;

    public Backup Backup { get; set; } = default!;
    public BackupFolder? ParentFolder { get; set; }
    public List<BackupFile> Files { get; set; } = new();
    public List<BackupFolder> Subfolders { get; set; } = new();
}
