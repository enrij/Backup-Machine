using BackupMachine.PoC.Domain.Aggregates;
using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Domain.Enums;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

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

    public CreateDatabaseDataFromFolderHandler(IMediator mediatr, IDbContextFactory<BackupMachineContext> dbContextFactory)
    {
        _mediatr = mediatr;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<BackupFolder> Handle(CreateDatabaseDataFromFolderCommand request, CancellationToken cancellationToken)
    {
        var destinationEntity = await _mediatr.Send(new CreateBackupFolderEntityCommand(request.Backup, request.Source), cancellationToken);

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var previousBackupFiles = context.Folders
                                         .Where(folder => request.Backup.PreviousBackupId != null && folder.BackupId == request.Backup.PreviousBackupId)
                                         .Include(folder => folder.Files)
                                         .AsEnumerable()
                                         .Where(folder => folder.RelativePath == Utilities.GetPathRelativeToJobSource(request.Source, request.Backup.Job))
                                         .SelectMany(folder => folder.Files.Select(file => file.Name))
                                         .Distinct()
                                         .ToList();

        var filesToBackup = new List<FileToBackup>();

        if (previousBackupFiles.Count > 0)
        {
            // New or updated files
            filesToBackup = filesToBackup.Concat(CompareActualFilesWithBackupAndReturnNewOrUpdatedFiles(
                                              request.Source.GetFiles().ToList(),
                                              previousBackupFiles))
                                         .ToList();

            // Deleted files
            filesToBackup = filesToBackup.Concat(CompareActualFilesWithBackupAndReturnDeletedFiles(
                                              request.Source.GetFiles().ToList(),
                                              previousBackupFiles.Select(file => Path.Combine(request.Source.FullName, file)).ToList()))
                                         .ToList();
        }
        else
        {
            filesToBackup = request.Source
                                   .GetFiles()
                                   .Select(file => new FileToBackup(file, FileStatus.New))
                                   .TakeWhile(_ => cancellationToken.IsCancellationRequested == false)
                                   .ToList();
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

    private List<FileToBackup> CompareActualFilesWithBackupAndReturnNewOrUpdatedFiles(List<FileInfo> actualFiles, List<string> backupFiles)
    {
        return actualFiles.GroupJoin(
                               backupFiles,
                               outer => outer.Name,
                               inner => inner,
                               (outer, inner) => new { File = outer, AlreadyBackedup = inner.Any() }
                           )
                          .Select(group => new FileToBackup(
                               group.File,
                               group.AlreadyBackedup ? FileStatus.Updated : FileStatus.New)
                           )
                          .ToList();
    }

    private List<FileToBackup> CompareActualFilesWithBackupAndReturnDeletedFiles(List<FileInfo> actualFiles, List<string> backupFiles)
    {
        return backupFiles.GroupJoin(
                               actualFiles,
                               outer => outer,
                               inner => inner.Name,
                               (outer, inner) => new { File = outer, Deleted = inner.Any() == false }
                           )
                          .Where(group => @group.Deleted)
                          .Select(group => new FileToBackup(
                               new FileInfo(group.File),
                               FileStatus.Deleted)
                           )
                          .ToList();
    }
}
