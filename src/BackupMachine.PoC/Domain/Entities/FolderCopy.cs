namespace BackupMachine.PoC.Domain.Entities;

public class FolderCopy
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid SnapshotId { get; set; }
    public Guid? ParentFolderId { get; set; }
    public Guid? PreviousVersionId { get; set; }
    public string OriginalPath { get; set; } = string.Empty;

    public Snapshot Snapshot { get; set; } = default!;
    public FolderCopy? ParentFolder { get; set; }
    public FolderCopy? PreviousVersion { get; set; } = default!;
    public List<FolderCopy> Folders { get; set; } = new();
    public List<FileCopy> Files { get; set; } = new();
}
