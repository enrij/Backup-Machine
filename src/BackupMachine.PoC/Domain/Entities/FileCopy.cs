namespace BackupMachine.PoC.Domain.Entities;

public class FileCopy
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public Guid SnapshotId { get; set; }
    public Guid? FolderId { get; set; }
    public Guid? PreviousVersionId { get; set; }
    public string OriginalPath { get; set; } = string.Empty;

    public Snapshot Snapshot { get; set; } = default!;

    public FolderCopy? Folder { get; set; } = default!;
    public FileCopy? PreviousVersion { get; set; }
}
