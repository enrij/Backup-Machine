using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.PoC.Domain.Commands;

public class CleanFolderCommand : IRequest
{
    public CleanFolderCommand(DirectoryInfo folder)
    {
        Folder = folder;
    }

    public DirectoryInfo Folder { get; init; }
}

public class CleanFolderHandler : AsyncRequestHandler<CleanFolderCommand>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CleanFolderHandler> _logger;

    public CleanFolderHandler(IMediator mediator, ILogger<CleanFolderHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override async Task Handle(CleanFolderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Cleaning folder [{Path}]", request.Folder.FullName);
        
        if (request.Folder.GetFiles().Length > 0)
        {
            foreach (var file in request.Folder.GetFiles())
            {
                file.Delete();
                _logger.LogDebug("Deleted temporary file [{File}]", file.FullName);
            }

            _logger.LogDebug("Folder [{Path}] cleaned", request.Folder.FullName);
        }
        else
        {
            _logger.LogDebug("Folder [{Path}] has no files", request.Folder.FullName);
        }

        foreach (var directory in request.Folder.GetDirectories())
        {
            await _mediator.Send(new CleanFolderCommand(directory), cancellationToken);
        }
        
        request.Folder.Delete();
        _logger.LogDebug("Deleted folder [{Path}]", request.Folder.FullName);
    }
}
