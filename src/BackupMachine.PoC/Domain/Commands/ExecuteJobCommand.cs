using BackupMachine.PoC.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.PoC.Domain.Commands;

public class ExecuteJobCommand : IRequest
{
    public ExecuteJobCommand(Job job)
    {
        Job = job;
    }

    public Job Job { get; init; }
}

public class ExecuteJobHandler : AsyncRequestHandler<ExecuteJobCommand>
{
    private readonly ILogger<ExecuteJobHandler> _logger;
    private readonly IMediator _mediatr;

    public ExecuteJobHandler(ILogger<ExecuteJobHandler> logger, IMediator mediatr)
    {
        _logger = logger;
        _mediatr = mediatr;
    }

    protected override async Task Handle(ExecuteJobCommand request, CancellationToken cancellationToken)
    {
        var timestamp = DateTime.Now;

        /*
         Stage 1: Create temporary directory structure and copy files in it
         Stage 2: Create archives for each folder starting from the deepest ad bubbling up to the root
         Stage 3: Move the archives to the target directory
         Stage 4: Cleanup
         */

        _logger.LogDebug("Job [{Name}] started", request.Job.Name);

        var tempFolder = await _mediatr.Send(new CreateTempFolderCommand(), cancellationToken);

        tempFolder = new DirectoryInfo(Path.Combine(tempFolder.FullName, new DirectoryInfo(request.Job.Destination).Name));
        var source = new DirectoryInfo(request.Job.Source);
        var destination = new DirectoryInfo(request.Job.Destination);

        // Stage 1
        await _mediatr.Send(new CopyFolderCommand(source, tempFolder, timestamp), cancellationToken);

        // Stage 2
        await _mediatr.Send(new ZipFolderCommand(tempFolder, timestamp), cancellationToken);

        // Stage 3
        await _mediatr.Send(new MoveArchiveCommand(tempFolder, destination, timestamp), cancellationToken);
        
        // Stage 4
        await _mediatr.Send(new CleanFolderCommand(tempFolder), cancellationToken);
        await _mediatr.Send(new CleanFolderCommand(tempFolder.Parent!), cancellationToken);

        _logger.LogDebug("Job [{Name}] completed", request.Job.Name);
    }
}
