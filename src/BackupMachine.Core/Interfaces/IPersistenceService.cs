using BackupMachine.Core.Entities;

namespace BackupMachine.Core.Interfaces;

public interface IPersistenceService
{
    Task<Backup?> GetLatestBackupAsync(Job job, CancellationToken cancellationToken = default);
    Task<BackupFolder?> GetBackupFolderByPathAsync(string relativePath, Backup backup, CancellationToken cancellationToken = default);
    Task<List<BackupFolder>> GetAllFoldersForBackupAsync(Backup backup, CancellationToken cancellationToken = default);
    Task<List<BackupFile>> GetAllFilesForBackupAsync(Backup backup, CancellationToken cancellationToken = default);
    Task<Backup> CreateBackupAsync(Job backup, CancellationToken cancellationToken = default);
    Task<BackupFile> CreateBackupFileAsync(BackupFile file, CancellationToken cancellationToken = default);
    Task<BackupFolder> CreateBackupFolderAsync(BackupFolder folder, CancellationToken cancellationToken = default);
    Task<List<BackupFile>> GetPreviousBackupFilesAsync(Backup backup, DirectoryInfo source, CancellationToken cancellationToken = default);
    Task DeleteBackupAsync(Backup backup, CancellationToken cancellationToken = default);
    Task<BackupFolder> UpdateBackupFolderAsync(BackupFolder folder, CancellationToken cancellationToken = default);
}
