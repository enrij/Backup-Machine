﻿using BackupMachine.PoC.Domain.Enums;

namespace BackupMachine.PoC.Domain.Entities;

public class BackupFile
{
    public Guid Id { get; set; }
    public Guid BackupId { get; set; }
    public Guid BackupFolderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public FileStatus Status { get; set; }

    public virtual Backup Backup { get; set; } = default!;
    public virtual BackupFolder BackupFolder { get; set; } = default!;
}
