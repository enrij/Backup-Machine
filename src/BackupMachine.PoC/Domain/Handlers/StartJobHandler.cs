using BackupMachine.PoC.Domain.Commands;

using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.PoC.Domain.Handlers;

public class StartJobHandler : AsyncRequestHandler<StartJobCommand>
{
    private readonly ILogger<StartJobHandler> _logger;
    private readonly IMediator _mediatr;

    public StartJobHandler(ILogger<StartJobHandler> logger, IMediator mediatr)
    {
        _logger = logger;
        _mediatr = mediatr;
    }

    protected override async Task Handle(StartJobCommand request, CancellationToken cancellationToken)
    {
        var timestamp = DateTime.Now;
        
        _logger.LogDebug("Job [{Name}] started", request.Job.Name);
        
        var tempFolder = await _mediatr.Send(new CreateTempFolderCommand(), cancellationToken);
        
        _logger.LogDebug("Temporary worker folder [{Path}] created", tempFolder.FullName);
        
        var destinationFolder = new DirectoryInfo(Path.Combine(tempFolder.FullName, new DirectoryInfo(request.Job.Destination).Name));
        var sourceFolder = new DirectoryInfo(request.Job.Source);

        await _mediatr.Send(new CopyFolderCommand(sourceFolder, destinationFolder, timestamp), cancellationToken);
        _logger.LogDebug("Job [{Name}] completed", request.Job.Name);
    }
}
