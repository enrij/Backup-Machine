using BackupMachine.PoC.Domain.Enums;

namespace BackupMachine.PoC.Domain.Aggregates;

public record FileToBackup(
    FileInfo FileInfo,
    FileStatus Status
);
