using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.PoC.Domain.Commands;

public class CopyFolderCommand : IRequest
{
    public CopyFolderCommand(DirectoryInfo source, DirectoryInfo destination, DateTime timestamp)
    {
        Source = source;
        Destination = destination;
        Timestamp = timestamp;
    }

    public DirectoryInfo Source { get; init; }
    public DirectoryInfo Destination { get; init; }
    public DateTime Timestamp { get; init; }
}

public class CopyFolderHandler : AsyncRequestHandler<CopyFolderCommand>
{
    private readonly ILogger<CopyFolderHandler> _logger;
    private readonly IMediator _mediatr;

    public CopyFolderHandler(IMediator mediatr, ILogger<CopyFolderHandler> logger)
    {
        _mediatr = mediatr;
        _logger = logger;
    }

    protected override async Task Handle(CopyFolderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Copying folder {Folder}", request.Source.FullName);
        Directory.CreateDirectory(request.Destination.FullName);

        foreach (var file in request.Source.GetFiles().TakeWhile(_ => !cancellationToken.IsCancellationRequested))
        {
            _logger.LogDebug("Copying file {File}", file.FullName);
            File.Copy(file.FullName, Path.Combine(request.Destination.FullName, file.Name));
            _logger.LogDebug("Copied file {File}", file.FullName);
        }

        foreach (var directory in request.Source.GetDirectories().TakeWhile(_ => !cancellationToken.IsCancellationRequested))
        {
            var destination = Directory.CreateDirectory(Path.Combine(request.Destination.FullName, directory.Name));
            await _mediatr.Send(new CopyFolderCommand(directory, destination, request.Timestamp), cancellationToken);
        }

        _logger.LogDebug("Copied folder {Folder}", request.Source.FullName);
    }
}
