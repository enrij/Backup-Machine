using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackupMachine.PoC.Domain.Commands;

public class BackupFolderCommand : IRequest
{
    public BackupFolderCommand(DirectoryInfo source, DirectoryInfo destination, Backup backup)
    {
        Source = source;
        Destination = destination;
        Backup = backup;
    }

    public DirectoryInfo Source { get; init; }
    public DirectoryInfo Destination { get; init; }
    public Backup Backup { get; init; }
}

public class BackupFolderHandler : AsyncRequestHandler<BackupFolderCommand>
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;
    private readonly ILogger<BackupFolderHandler> _logger;
    private readonly IMediator _mediatr;

    public BackupFolderHandler(IMediator mediatr, ILogger<BackupFolderHandler> logger, IDbContextFactory<BackupMachineContext> dbContextFactory)
    {
        _mediatr = mediatr;
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task Handle(BackupFolderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Copying folder {Folder}", request.Source.FullName);
        Directory.CreateDirectory(request.Destination.FullName);

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var previousBackupFiles = context.Folders
                                         .Include(folder => folder.Files)
                                         .AsEnumerable()
                                         .Where(folder => folder.RelativePath == Utilities.GetPathRelativeToTemporaryFolder(request.Destination.FullName, request.Backup))
                                         .SelectMany(folder => folder.Files.Select(file => file.Name))
                                         .Distinct()
                                         .ToList();

        List<FileInfo> filesToBackup;

        if (previousBackupFiles.Count > 0)
        {
            filesToBackup = request.Source
                                   .GetFiles()
                                   .GroupJoin(
                                        previousBackupFiles,
                                        outer => outer.Name,
                                        inner => inner,
                                        (outer, inner) => new { File = outer, AlreadyBackedup = inner.Any() }
                                    )
                                   .Where(group => group.AlreadyBackedup == false)
                                   .Select(group => group.File)
                                   .TakeWhile(_ => cancellationToken.IsCancellationRequested == false)
                                   .ToList();
        }
        else
        {
            filesToBackup = request.Source
                                   .GetFiles()
                                   .ToList();
        }

        if (filesToBackup.Count == 0)
        {
            _logger.LogDebug("No files to backup in folder {Folder}", request.Source.FullName);
        }

        foreach (var file in filesToBackup)
        {
            _logger.LogDebug("Copying file {File}", file.FullName);
            File.Copy(file.FullName, Path.Combine(request.Destination.FullName, file.Name));
            _logger.LogDebug("Copied file {File}", file.FullName);
        }

        foreach (var directory in request.Source.GetDirectories().TakeWhile(_ => cancellationToken.IsCancellationRequested == false))
        {
            var destination = Directory.CreateDirectory(Path.Combine(request.Destination.FullName, directory.Name));
            await _mediatr.Send(new BackupFolderCommand(directory, destination, request.Backup), cancellationToken);
        }

        _logger.LogDebug("Copied folder {Folder}", request.Source.FullName);
    }
}
