using BackupMachine.Core.Entities;

namespace BackupMachine.Core.Interfaces;

public interface IPersistenceService
{
    Task<Backup?> GetLatestBackup(Job job, CancellationToken cancellationToken = default);
    Task<BackupFolder?> GetBackupFolderEntityByPath(string relativePath, Backup backup, CancellationToken cancellationToken = default);
    Task<List<BackupFolder>> GetAllFolderEntitiesForBackup(Backup backup, CancellationToken cancellationToken = default);
    Task<List<BackupFile>> GetAllFileEntitiesForBackup(Backup backup, CancellationToken cancellationToken = default);
    Task<Backup> CreateBackupEntity(Backup backup, CancellationToken cancellationToken = default);
    Task<BackupFile> CreateBackupFile(BackupFile file, CancellationToken cancellationToken = default);
    Task<BackupFolder> CreateBackupFolderEntity(BackupFolder folder, CancellationToken cancellationToken = default);
    Task<List<BackupFile>> GetPreviousBackupFileEntities(Backup backup, DirectoryInfo source, CancellationToken cancellationToken = default);
    Task DeleteBackupEntity(Backup backup, CancellationToken cancellationToken = default);
    Task<BackupFolder> UpdateBackupFolderAsync(BackupFolder folder, CancellationToken cancellationToken = default);
}
