namespace BackupMachine.PoC.Domain.Entities;

public class BackupFolder
{
    public Guid Id { get; set; }
    public Guid BackupId { get; set; }
    public Guid? ParentFolderId { get; set; }
    public DirectoryInfo Source { get; set; } = default!;
    public DirectoryInfo Destination { get; set; } = default!;

    public Backup Backup { get; set; } = default!;
    public BackupFolder? ParentFolder { get; set; }
    public List<BackupFile> Files { get; set; } = new();
    public List<BackupFolder> Subfolders { get; set; } = new();
}
