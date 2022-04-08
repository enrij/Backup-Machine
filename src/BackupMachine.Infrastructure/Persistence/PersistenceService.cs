using BackupMachine.Core;
using BackupMachine.Core.Entities;
using BackupMachine.Core.Enums;
using BackupMachine.Core.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.Infrastructure.Persistence;

public class PersistenceService : IPersistenceService
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;

    public PersistenceService(IDbContextFactory<BackupMachineContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Backup?> GetLatestBackupAsync(Job job, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Backups
                            .Where(backup => backup.JobId == job.Id)
                            .OrderByDescending(backup => backup.Timestamp)
                            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<BackupFolder?> GetBackupFolderByPathAsync(string relativePath, Backup backup, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return context.Folders
                      .Where(folder => folder.BackupId == backup.Id)
                      .Include(folder => folder.Files)
                      .AsEnumerable()
                      .FirstOrDefault(
                           folder => folder.RelativePath == relativePath);
    }

    public async Task<List<BackupFolder>> GetAllFoldersForBackupAsync(Backup backup, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Folders
                            .Where(folder => folder.BackupId == backup.Id)
                            .Include(folder => folder.Backup)
                            .ThenInclude(folderBackup => folderBackup.Job)
                            .ToListAsync(cancellationToken);
    }

    public async Task<List<BackupFile>> GetAllFilesForBackupAsync(Backup backup, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Files
                            .Where(file => file.BackupId == backup.Id)
                            .Include(file => file.Backup)
                            .ThenInclude(fileBackup => fileBackup.Job)
                            .Include(file => file.Backup)
                            .ThenInclude(fileBackup => fileBackup.PreviousBackup)
                            .Include(file => file.BackupFolder)
                            .ToListAsync(cancellationToken);
    }

    public async Task<Backup> CreateBackupAsync(Backup backup, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        context.Entry(backup).State = EntityState.Added;
        await context.SaveChangesAsync(cancellationToken);

        return backup;
    }

    public async Task<BackupFile> CreateBackupFileAsync(BackupFile file, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        context.Entry(file).State = EntityState.Added;
        await context.SaveChangesAsync(cancellationToken);

        return file;
    }

    public async Task<BackupFolder> CreateBackupFolderAsync(BackupFolder folder, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        context.Entry(folder).State = EntityState.Added;
        await context.SaveChangesAsync(cancellationToken);

        return folder;
    }

    public async Task<List<BackupFile>> GetPreviousBackupFilesAsync(Backup backup, DirectoryInfo source, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var previousBackupFiles = context.Files
                                         .Where(file =>
                                              backup.PreviousBackupId != null &&
                                              file.BackupId == backup.PreviousBackupId &&
                                              file.Status != FileStatus.Deleted)
                                         .Include(file => file.BackupFolder)
                                         .Include(file => file.Backup)
                                         .ThenInclude(backup => backup.Job)
                                         .AsEnumerable()
                                         .Where(file => file.BackupFolder.RelativePath == Utilities.GetPathRelativeToJobSource(source, backup.Job))
                                         .Select(file => file)
                                         .Distinct()
                                         .ToList();
        return previousBackupFiles;
    }

    public async Task DeleteBackupAsync(Backup backup, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var filesToDelete = context.Files
                                   .Where(file => file.BackupId == backup.Id)
                                   .ToList();

        var foldersToDelete = context.Folders
                                     .Where(file => file.BackupId == backup.Id)
                                     .ToList();

        context.RemoveRange(filesToDelete);
        context.RemoveRange(foldersToDelete);
        context.Remove(backup);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<BackupFolder> UpdateBackupFolderAsync(BackupFolder folder, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        context.Update(folder);
        await context.SaveChangesAsync(cancellationToken);

        return folder;
    }
}
