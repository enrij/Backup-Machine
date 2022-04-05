using BackupMachine.PoC.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.PoC.Domain.Commands;

public class BackupFolderCommand : IRequest<BackupFolder>
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

public class BackupFolderHandler : IRequestHandler<BackupFolderCommand, BackupFolder>
{
    private readonly ILogger<BackupFolderHandler> _logger;
    private readonly IMediator _mediatr;

    public BackupFolderHandler(IMediator mediatr, ILogger<BackupFolderHandler> logger)
    {
        _mediatr = mediatr;
        _logger = logger;
    }

    public async Task<BackupFolder> Handle(BackupFolderCommand request, CancellationToken cancellationToken)
    {
        var destinationEntity = await _mediatr.Send(new CreateBackupFolderEntityCommand(request.Backup, request.Source, request.Destination), cancellationToken);
        
        _logger.LogDebug("Copying folder {Folder}", request.Source.FullName);
        Directory.CreateDirectory(request.Destination.FullName);

        foreach (var file in request.Source.GetFiles().TakeWhile(_ => cancellationToken.IsCancellationRequested == false))
        {
            _logger.LogDebug("Copying file {File}", file.FullName);
            File.Copy(file.FullName, Path.Combine(request.Destination.FullName, file.Name));
            _logger.LogDebug("Copied file {File}", file.FullName);

            var fileEntity = await _mediatr.Send(new CreateBackupFileEntityCommand(request.Backup, destinationEntity, file), cancellationToken);
            destinationEntity.Files.Add(fileEntity);
        }

        foreach (var directory in request.Source.GetDirectories().TakeWhile(_ => cancellationToken.IsCancellationRequested == false))
        {
            var destination = Directory.CreateDirectory(Path.Combine(request.Destination.FullName, directory.Name));
            var subfolderEntity = await _mediatr.Send(new BackupFolderCommand(directory, destination, request.Backup), cancellationToken);
            destinationEntity.Subfolders.Add(subfolderEntity);
        }
        
        if (destinationEntity.Files.Count > 0 || destinationEntity.Subfolders.Count > 0)
        {
            destinationEntity = await _mediatr.Send(new UpdateBackupFolderCommand(destinationEntity), cancellationToken);
        }

        _logger.LogDebug("Copied folder {Folder}", request.Source.FullName);

        return destinationEntity;
    }
}
