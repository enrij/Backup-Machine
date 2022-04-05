using BackupMachine.PoC.Domain.Entities;

using MediatR;

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
    private readonly ILogger<BackupFolderHandler> _logger;
    private readonly IMediator _mediatr;

    public BackupFolderHandler(IMediator mediatr, ILogger<BackupFolderHandler> logger)
    {
        _mediatr = mediatr;
        _logger = logger;
    }

    protected override async Task Handle(BackupFolderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Copying folder {Folder}", request.Source.FullName);
        Directory.CreateDirectory(request.Destination.FullName);

        foreach (var file in request.Source.GetFiles().TakeWhile(_ => cancellationToken.IsCancellationRequested == false))
        {
            _logger.LogDebug("Copying file {File}", file.FullName);
            File.Copy(file.FullName, Path.Combine(request.Destination.FullName, file.Name));
            _logger.LogDebug("Copied file {File}", file.FullName);
        }

        foreach (var directory in request.Source.GetDirectories().TakeWhile(_ => cancellationToken.IsCancellationRequested == false))
        {
            var destination = Directory.CreateDirectory(Path.Combine(request.Destination.FullName, directory.Name));
            var subfolderEntity = await _mediatr.Send(new BackupFolderCommand(directory, destination, request.Backup), cancellationToken);
        }

        _logger.LogDebug("Copied folder {Folder}", request.Source.FullName);
    }
}
