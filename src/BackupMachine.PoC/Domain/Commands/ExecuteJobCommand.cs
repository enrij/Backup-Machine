﻿using BackupMachine.PoC.Domain.Entities;

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
        /*
         Stage 1: Create database entries
         Stage 2: Create folders structure at destination
         */

        _logger.LogDebug("Job [{Name}] started", request.Job.Name);

        // Stage 1
        var backup = await _mediatr.Send(new CreateBackupEntityCommand(request.Job), cancellationToken);
        var source = new DirectoryInfo(request.Job.Source);
        var destination = Directory.CreateDirectory(Path.Combine(request.Job.Destination, Utilities.GetBackupDestinationRootFolderPath(backup)));

        // Stage 1
        await _mediatr.Send(new CreateDatabaseDataFromFolderCommand(backup, source), cancellationToken);

        // Stage 2
        await _mediatr.Send(new CreateDestinationFolderStructureCommand(backup), cancellationToken);

        // Stage 3
        await _mediatr.Send(new BackupFilesCommand(backup), cancellationToken);

        _logger.LogDebug("Job [{Name}] completed", request.Job.Name);
    }
}
