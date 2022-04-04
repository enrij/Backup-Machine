using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.PoC.Domain.Commands;

public class MoveArchiveCommand : IRequest
{
    public MoveArchiveCommand(DirectoryInfo source, DirectoryInfo destination, DateTime timeStamp)
    {
        Destination = destination;
        TimeStamp = timeStamp;
        Source = source;
    }

    public DirectoryInfo Source { get; init; }
    public DirectoryInfo Destination { get; init; }
    public DateTime TimeStamp { get; init; }
}

public class MoveArchiveHandler : AsyncRequestHandler<MoveArchiveCommand>
{
    private readonly IMediator _mediator;
    private readonly ILogger<MoveArchiveHandler> _logger;

    public MoveArchiveHandler(IMediator mediator, ILogger<MoveArchiveHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override async Task Handle(MoveArchiveCommand request, CancellationToken cancellationToken)
    {
        if (request.Destination.Exists == false)
        {
            request.Destination.Create();

            _logger.LogDebug("Created destination folder [{Path}]", request.Destination.FullName);
        }
        
        var archiveFile = request.Source
                                 .GetFiles()
                                 .FirstOrDefault(file => file.Name == $"BackupMachine-{request.TimeStamp:yyyy MM dd HH mm ss}.zip");

        if (archiveFile is not null)
        {
            archiveFile.MoveTo(Path.Combine(request.Destination.FullName, archiveFile.Name));
            _logger.LogDebug("Archive [{Archive}] moved to destination", archiveFile.FullName);
        }

        foreach (var directory in request.Source.GetDirectories())
        {
            await _mediator.Send(
                new MoveArchiveCommand(
                    directory, 
                    new DirectoryInfo(Path.Combine(request.Destination.FullName, directory.Name)),
                    request.TimeStamp),
                cancellationToken);
        }
    }
}
