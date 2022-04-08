using BackupMachine.Core.Entities;
using BackupMachine.Core.Enums;
using BackupMachine.Core.Interfaces;
using BackupMachine.PoC.Domain.Aggregates;

using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.Core.Commands;

public class CreateDatabaseDataFromFolderCommand : IRequest<BackupFolder>
{
    public CreateDatabaseDataFromFolderCommand(Backup backup, DirectoryInfo source)
    {
        Backup = backup;
        Source = source;
    }

    public DirectoryInfo Source { get; init; }
    public Backup Backup { get; init; }
}

public class CreateDatabaseDataFromFolderHandler : IRequestHandler<CreateDatabaseDataFromFolderCommand, BackupFolder>
{
    private readonly ILogger<CreateDatabaseDataFromFolderHandler> _logger;
    private readonly IMediator _mediatr;
    private readonly IPersistenceService _persistenceService;

    public CreateDatabaseDataFromFolderHandler(IMediator mediatr, ILogger<CreateDatabaseDataFromFolderHandler> logger, IPersistenceService persistenceService)
    {
        _mediatr = mediatr;
        _logger = logger;
        _persistenceService = persistenceService;
    }

    public async Task<BackupFolder> Handle(CreateDatabaseDataFromFolderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Analyzing folder [{Folder}]", request.Source.FullName);
        
        var destinationEntity = await CreateDestinationEntity(request, cancellationToken);

        var previousBackupFiles = (await _persistenceService.GetPreviousBackupFilesAsync(request.Backup, request.Source, cancellationToken))
                                 .Where(file => file.Status != FileStatus.Deleted)
                                 .ToList();

        var filesToBackup = CompareActualFilesToBackup(request.Source, previousBackupFiles);

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
            var file = await CreateFileEntity(request, cancellationToken, fileToBackup, destinationEntity);

            destinationEntity.Files.Add(file);
        }

        foreach (var directory in request.Source.GetDirectories().TakeWhile(_ => cancellationToken.IsCancellationRequested == false))
        {
            var subfolderEntity = await _mediatr.Send(new CreateDatabaseDataFromFolderCommand(request.Backup, directory), cancellationToken);
            destinationEntity.Subfolders.Add(subfolderEntity);
        }

        if (filesToBackup.Count > 0 || destinationEntity.Subfolders.Count > 0)
        {
            destinationEntity = await _persistenceService.UpdateBackupFolderAsync(destinationEntity, cancellationToken);
        }

        return destinationEntity;
    }

    private async Task<BackupFile> CreateFileEntity(CreateDatabaseDataFromFolderCommand request, CancellationToken cancellationToken, FileToBackup fileToBackup, BackupFolder destinationEntity)
    {
        var file = new BackupFile
        {
            Backup = request.Backup,
            Name = fileToBackup.FileInfo.Name,
            Extension = fileToBackup.FileInfo.Extension,
            BackupFolder = destinationEntity,
            Status = fileToBackup.Status,
            Length = fileToBackup.FileInfo.Length,
            Modified = fileToBackup.FileInfo.LastWriteTimeUtc,
            Created = fileToBackup.FileInfo.CreationTimeUtc
        };

        file = await _persistenceService.CreateBackupFileAsync(file, cancellationToken);
        return file;
    }

    private async Task<BackupFolder> CreateDestinationEntity(CreateDatabaseDataFromFolderCommand request, CancellationToken cancellationToken)
    {
        var destinationEntity = new BackupFolder
        {
            Backup = request.Backup,
            RelativePath = Utilities.GetRelativePathFromJobSource(request.Source, request.Backup.Job),
            ParentFolder =
                request.Source.Parent is null
                    ? null
                    : await _persistenceService.GetBackupFolderByPathAsync(request.Source.Parent.FullName, request.Backup, cancellationToken)
        };

        destinationEntity = await _persistenceService.CreateBackupFolderAsync(destinationEntity, cancellationToken);
        return destinationEntity;
    }

    private List<FileToBackup> CompareActualFilesToBackup(DirectoryInfo source, List<BackupFile> previousBackupFiles)
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

    private List<FileToBackup> CompareActualFolderFilesWithBackupAndReturnNewFiles(List<FileInfo> actualFiles, List<BackupFile> backupFiles)
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

    private List<FileToBackup> CompareActualFolderFilesWithBackupAndReturnUpdatedFiles(List<FileInfo> actualFiles, List<BackupFile> backupFiles)
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

    private List<FileToBackup> CompareActualFolderFilesWithBackupAndReturnUnchangedFiles(List<FileInfo> actualFiles, List<BackupFile> backupFiles)
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

    private List<FileToBackup> CompareActualFolderFilesWithBackupAndReturnDeletedFiles(List<FileInfo> actualFiles, List<BackupFile> backupFiles)
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
}
