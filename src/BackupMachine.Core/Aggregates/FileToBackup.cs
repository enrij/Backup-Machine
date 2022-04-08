using BackupMachine.Core.Enums;

namespace BackupMachine.PoC.Domain.Aggregates;

public record FileToBackup(
    FileInfo FileInfo,
    FileStatus Status
);
