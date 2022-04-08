using BackupMachine.Core.Enums;

namespace BackupMachine.Core.Aggregates;

public record FileToBackup(
    FileInfo FileInfo,
    FileStatus Status
);
