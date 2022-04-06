using BackupMachine.PoC.Domain.Aggregates;
using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Domain.Enums;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackupMachine.PoC.Domain.Commands;

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
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;
    private readonly IMediator _mediatr;
    private readonly ILogger<CreateDatabaseDataFromFolderHandler> _logger;

    public CreateDatabaseDataFromFolderHandler(IMediator mediatr, IDbContextFactory<BackupMachineContext> dbContextFactory, ILogger<CreateDatabaseDataFromFolderHandler> logger)
    {
        _mediatr = mediatr;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task<BackupFolder> Handle(CreateDatabaseDataFromFolderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Analyzing folder [{Folder}]", request.Source.FullName);

        var destinationEntity = await _mediatr.Send(new CreateBackupFolderEntityCommand(request.Backup, request.Source), cancellationToken);

        var previousBackupFiles = await GetPreviousBackupFileEntities(request, cancellationToken);

        List<FileToBackup> filesToBackup;

        if (previousBackupFiles.Count > 0)
        {
            filesToBackup = CompareActualFilesToBackup(request.Source, previousBackupFiles);
        }
        else
        {
            filesToBackup = request.Source
                                   .GetFiles()
                                   .Select(file => new FileToBackup(file, FileStatus.New))
                                   .TakeWhile(_ => cancellationToken.IsCancellationRequested == false)
                                   .ToList();
        }

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
            var fileEntity = await _mediatr.Send(new CreateBackupFileEntityCommand(request.Backup, destinationEntity, fileToBackup), cancellationToken);
            destinationEntity.Files.Add(fileEntity);
        }

        foreach (var directory in request.Source.GetDirectories().TakeWhile(_ => cancellationToken.IsCancellationRequested == false))
        {
            var subfolderEntity = await _mediatr.Send(new CreateDatabaseDataFromFolderCommand(request.Backup, directory), cancellationToken);
            destinationEntity.Subfolders.Add(subfolderEntity);
        }

        if (filesToBackup.Count > 0 || destinationEntity.Subfolders.Count > 0)
        {
            destinationEntity = await _mediatr.Send(new UpdateBackupFolderCommand(destinationEntity), cancellationToken);
        }

        return destinationEntity;
    }

    private async Task<List<BackupFile>> GetPreviousBackupFileEntities(CreateDatabaseDataFromFolderCommand request, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var previousBackupFiles = context.Files
                                         .Where(file => 
                                              request.Backup.PreviousBackupId != null && 
                                              file.BackupId == request.Backup.PreviousBackupId)
                                         .Include(file => file.BackupFolder)
                                         .Include(file => file.Backup)
                                         .ThenInclude(backup => backup.Job)
                                         .AsEnumerable()
                                         .Where(file => file.BackupFolder.RelativePath == Utilities.GetPathRelativeToJobSource(request.Source, request.Backup.Job))
                                         .Select(file => file)
                                         .Distinct()
                                         .ToList();
        return previousBackupFiles;
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
                               group.File.LastWriteTimeUtc != group.BackupFile.Modified&&
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
                               new FileInfo(Utilities.GetBackupFileDestinationPath(group.File)),
                               FileStatus.Deleted)
                           )
                          .ToList();
    }
}
