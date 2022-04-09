using BackupMachine.Core.Aggregates;
using BackupMachine.Core.Entities;
using BackupMachine.Core.Enums;
using BackupMachine.Core.Interfaces;

using Microsoft.Extensions.Logging;

namespace BackupMachine.Core.Services;

public class BackupsService
{
    private readonly IFileSystemService _fileSystemService;
    private readonly ILogger<BackupsService> _logger;
    private readonly IPersistenceService _persistenceService;


    public BackupsService(ILogger<BackupsService> logger, IFileSystemService fileSystemService, IPersistenceService persistenceService)
    {
        _logger = logger;
        _fileSystemService = fileSystemService;
        _persistenceService = persistenceService;
    }

    private void MoveFileFromPreviousBackupToCurrentBackup(BackupFile file)
    {
        if (file.Backup.PreviousBackup is null)
        {
            throw new InvalidDataException("File has no previous backup");
        }

        var source = Utilities.GetBackupFileDestinationPath(file, file.Backup.PreviousBackup);
        var destination = Utilities.GetBackupFileDestinationPath(file);

        if (!source.Exists)
        {
            _logger.LogWarning("File [{File}] not found", source.FullName);
            return;
        }

        _fileSystemService.MoveFile(source, destination);
    }

    private void CopyFileFromSourceIntoCurrentBackup(BackupFile file)
    {
        var source = Utilities.GetBackupFileSourceInfo(file);
        var destination = Utilities.GetBackupFileDestinationPath(file);

        if (!source.Exists)
        {
            _logger.LogWarning("File [{File}] not found", source.FullName);
            return;
        }

        _fileSystemService.CopyFile(source, destination);

        _logger.LogDebug("File [{File}] copied", source.FullName);
    }

    private async Task BackupFilesAsync(Backup backup, CancellationToken cancellationToken = default)
    {
        var filesInBackup = await _persistenceService.GetAllFilesForBackupAsync(backup, cancellationToken);
        foreach (var file in filesInBackup.TakeWhile(_ => cancellationToken.IsCancellationRequested == false))
        {
            Console.WriteLine($"Cancellation requested {cancellationToken.IsCancellationRequested}");
            
            _logger.LogDebug("Processing file [{File}]", file.Name);

            switch (file.Status)
            {
                case FileStatus.New:
                    CopyFileFromSourceIntoCurrentBackup(file);
                    break;
                case FileStatus.Updated:
                    CopyFileFromSourceIntoCurrentBackup(file);
                    break;
                case FileStatus.Unchanged:
                    MoveFileFromPreviousBackupToCurrentBackup(file);
                    break;
                case FileStatus.Deleted:
                    break;
                default:
                    throw new Exception("Invalid file status"); // Why are you here????
            }
        }
    }

    private async Task CreateDestinationBackupFolderStructureAsync(Backup backup, CancellationToken cancellationToken = default)
    {
        var foldersInBackup = await _persistenceService.GetAllFoldersForBackupAsync(backup, cancellationToken);
        foreach (var folder in foldersInBackup.TakeWhile(_ => cancellationToken.IsCancellationRequested == false))
        {
            _fileSystemService.CreateDirectory(Utilities.GetBackupFolderDestinationPath(folder));
        }
    }

    private async Task<BackupFolder> CreateDatabaseDataFromFolderAsync(DirectoryInfo source, Backup backup, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Analyzing folder [{Folder}]", source.FullName);

        var destinationEntity = await CreateDestinationEntityAsync(source, backup, cancellationToken);

        var previousBackupFiles = (await _persistenceService.GetPreviousBackupFilesAsync(backup, source, cancellationToken))
                                 .Where(file => file.Status != FileStatus.Deleted)
                                 .ToList();

        var filesToBackup = CompareActualFilesToBackup(source, previousBackupFiles);

        if (filesToBackup.Any(file => file.Status != FileStatus.Unchanged))
        {
            _logger.LogInformation("Since previous backup: {New} new files, {Updated} updated files and {Deleted} deleted files ",
                filesToBackup.Count(file => file.Status == FileStatus.New),
                filesToBackup.Count(file => file.Status == FileStatus.Updated),
                filesToBackup.Count(file => file.Status == FileStatus.Deleted));
        }
        else
        {
            _logger.LogInformation("No changes since previous backup");
        }

        foreach (var fileToBackup in filesToBackup.TakeWhile(_ => cancellationToken.IsCancellationRequested == false))
        {
            var file = await CreateFileEntityAsync(backup, fileToBackup, destinationEntity, cancellationToken);

            destinationEntity.Files.Add(file);
        }

        foreach (var directory in source.GetDirectories().TakeWhile(_ => cancellationToken.IsCancellationRequested == false))
        {
            var subfolderEntity = await CreateDatabaseDataFromFolderAsync(directory, backup, cancellationToken);
            destinationEntity.Subfolders.Add(subfolderEntity);
        }

        if (filesToBackup.Count > 0 || destinationEntity.Subfolders.Count > 0)
        {
            destinationEntity = await _persistenceService.UpdateBackupFolderAsync(destinationEntity, cancellationToken);
        }

        return destinationEntity;
    }

    private async Task<BackupFile> CreateFileEntityAsync(Backup backup, FileToBackup fileToBackup, BackupFolder destinationEntity, CancellationToken cancellationToken)
    {
        var (info, status) = fileToBackup;
        var file = new BackupFile
        {
            Backup = backup,
            Name = info.Name,
            Extension = info.Extension,
            BackupFolder = destinationEntity,
            Status = status,
            Length = info.Length,
            Modified = info.LastWriteTimeUtc,
            Created = info.CreationTimeUtc
        };

        file = await _persistenceService.CreateBackupFileAsync(file, cancellationToken);
        return file;
    }

    private async Task<BackupFolder> CreateDestinationEntityAsync(DirectoryInfo source, Backup backup, CancellationToken cancellationToken = default)
    {
        var destinationEntity = new BackupFolder
        {
            Backup = backup,
            RelativePath = Utilities.GetRelativePathFromJobSource(source, backup.Job),
            ParentFolder =
                source.Parent is null
                    ? null
                    : await _persistenceService.GetBackupFolderByPathAsync(source.Parent.FullName, backup, cancellationToken)
        };

        destinationEntity = await _persistenceService.CreateBackupFolderAsync(destinationEntity, cancellationToken);
        return destinationEntity;
    }

    private static List<FileToBackup> CompareActualFilesToBackup(DirectoryInfo source, IReadOnlyCollection<BackupFile> previousBackupFiles)
    {
        var filesToBackup = new List<FileToBackup>();
        // New files
        filesToBackup = filesToBackup.Concat(CompareActualFolderFilesWithBackupAndReturnNewFiles(
                                          source.GetFiles().ToList(),
                                          previousBackupFiles))
                                     .ToList();
        // Updated files
        filesToBackup = filesToBackup.Concat(CompareActualFolderFilesWithBackupAndReturnUpdatedFiles(
                                          source.GetFiles().ToList(),
                                          previousBackupFiles))
                                     .ToList();

        // Deleted files
        filesToBackup = filesToBackup.Concat(CompareActualFolderFilesWithBackupAndReturnDeletedFiles(
                                          source.GetFiles().ToList(),
                                          previousBackupFiles))
                                     .ToList();
        // Unchanged files
        filesToBackup = filesToBackup.Concat(CompareActualFolderFilesWithBackupAndReturnUnchangedFiles(
                                          source.GetFiles().ToList(),
                                          previousBackupFiles))
                                     .ToList();
        return filesToBackup;
    }

    private static IEnumerable<FileToBackup> CompareActualFolderFilesWithBackupAndReturnNewFiles(IEnumerable<FileInfo> actualFiles, IEnumerable<BackupFile> backupFiles)
    {
        return actualFiles.GroupJoin(
                               backupFiles,
                               outer => outer.Name,
                               inner => inner.Name,
                               (outer, inner) => new { File = outer, AlreadyBackedup = inner.Any() }
                           )
                          .Where(group => group.AlreadyBackedup == false)
                          .Select(group => new FileToBackup(
                               group.File,
                               FileStatus.New)
                           )
                          .ToList();
    }

    private static IEnumerable<FileToBackup> CompareActualFolderFilesWithBackupAndReturnUpdatedFiles(IEnumerable<FileInfo> actualFiles, IEnumerable<BackupFile> backupFiles)
    {
        return actualFiles.Join(
                               backupFiles,
                               outer => outer.Name,
                               inner => inner.Name,
                               (outer, inner) => new { File = outer, BackupFile = inner }
                           )
                          .Where(group =>
                               group.File.Length != group.BackupFile.Length ||
                               group.File.LastWriteTimeUtc != group.BackupFile.Modified &&
                               group.File.CreationTimeUtc != group.BackupFile.Created)
                          .Select(group => new FileToBackup(
                               group.File,
                               FileStatus.Updated)
                           )
                          .ToList();
    }

    private static IEnumerable<FileToBackup> CompareActualFolderFilesWithBackupAndReturnUnchangedFiles(IEnumerable<FileInfo> actualFiles, IEnumerable<BackupFile> backupFiles)
    {
        return actualFiles.Join(
                               backupFiles,
                               outer => outer.Name,
                               inner => inner.Name,
                               (outer, inner) => new { File = outer, BackupFile = inner }
                           )
                          .Where(group =>
                               group.File.Length == group.BackupFile.Length &&
                               group.File.LastWriteTimeUtc == group.BackupFile.Modified &&
                               group.File.CreationTimeUtc == group.BackupFile.Created)
                          .Select(group => new FileToBackup(
                               group.File,
                               FileStatus.Unchanged)
                           )
                          .ToList();
    }

    private static IEnumerable<FileToBackup> CompareActualFolderFilesWithBackupAndReturnDeletedFiles(IEnumerable<FileInfo> actualFiles, IEnumerable<BackupFile> backupFiles)
    {
        return backupFiles.GroupJoin(
                               actualFiles,
                               outer => outer.Name,
                               inner => inner.Name,
                               (outer, inner) => new { File = outer, Deleted = inner.Any() == false }
                           )
                          .Where(group => group.Deleted)
                          .Select(group => new FileToBackup(
                               Utilities.GetBackupFileDestinationPath(group.File),
                               FileStatus.Deleted)
                           )
                          .ToList();
    }

    public async Task ExecuteJobBackupAsync(Job job, CancellationToken cancellationToken = default)
    {
        /*
         Stage 1: Create database entries
         Stage 2: Create folders structure at destination
         Stage 3: Copy files from source to destination
         */

        _logger.LogDebug("Job [{Name}] started", job.Name);
        // Stage 1
        var backup = await _persistenceService.CreateBackupAsync(job, cancellationToken);

        try
        {
            var source = new DirectoryInfo(job.Source);

            // Stage 1
            await CreateDatabaseDataFromFolderAsync(source, backup, cancellationToken);

            // Stage 2
            await CreateDestinationBackupFolderStructureAsync(backup, cancellationToken);

            // Stage 3
            await BackupFilesAsync(backup, cancellationToken);

            _logger.LogDebug("Job [{Name}] completed", job.Name);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Job [{Name}] failed. Cleaning invalid data", job.Name);

            await _persistenceService.DeleteBackupAsync(backup, cancellationToken);

            _fileSystemService.DeleteFolderAndContent(Utilities.GetBackupDestinationRootFolderPath(backup));
        }
    }
}
