using BackupMachine.Core.Entities;
using BackupMachine.Core.Interfaces;

using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.Core.Commands;

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
    private readonly IFileSystemService _fileSystemService;
    private readonly ILogger<ExecuteJobHandler> _logger;
    private readonly IMediator _mediatr;
    private readonly IPersistenceService _persistenceService;

    public ExecuteJobHandler(ILogger<ExecuteJobHandler> logger, IMediator mediatr, IPersistenceService persistenceService, IFileSystemService fileSystemService)
    {
        _logger = logger;
        _mediatr = mediatr;
        _persistenceService = persistenceService;
        _fileSystemService = fileSystemService;
    }

    protected override async Task Handle(ExecuteJobCommand request, CancellationToken cancellationToken)
    {
        /*
         Stage 1: Create database entries
         Stage 2: Create folders structure at destination
         Stage 3: Copy files from source to destination
         */

        _logger.LogDebug("Job [{Name}] started", request.Job.Name);
        // Stage 1
        var backup = await _persistenceService.CreateBackupAsync(request.Job, cancellationToken);

        try
        {
            var source = new DirectoryInfo(request.Job.Source);

            // Stage 1
            await _mediatr.Send(new CreateDatabaseDataFromFolderCommand(backup, source), cancellationToken);

            // Stage 2
            await _mediatr.Send(new CreateDestinationFolderStructureCommand(backup), cancellationToken);

            // Stage 3
            await _mediatr.Send(new BackupFilesCommand(backup), cancellationToken);

            _logger.LogDebug("Job [{Name}] completed", request.Job.Name);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Job [{Name}] failed. Cleaning invalid data", request.Job.Name);

            await _persistenceService.DeleteBackupAsync(backup, cancellationToken);

            _fileSystemService.DeleteFolder(Utilities.GetBackupDestinationRootFolderPath(backup));
        }
    }
}
