using BackupMachine.Core.Entities;
using BackupMachine.Core.Enums;
using BackupMachine.Core.Queries;

using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.Core.Commands;

public class BackupFilesCommand : IRequest
{
    public BackupFilesCommand(Backup backup)
    {
        Backup = backup;
    }

    public Backup Backup { get; set; }
}

public class BackupFilesHandler : AsyncRequestHandler<BackupFilesCommand>
{
    private readonly ILogger<BackupFilesHandler> _logger;
    private readonly IMediator _mediatr;

    public BackupFilesHandler(IMediator mediatr, ILogger<BackupFilesHandler> logger)
    {
        _mediatr = mediatr;
        _logger = logger;
    }

    protected override async Task Handle(BackupFilesCommand request, CancellationToken cancellationToken)
    {
        var filesInBackup = await _mediatr.Send(new GetAllFileEntitiesForBackupQuery(request.Backup), cancellationToken);
        foreach (var file in filesInBackup)
        {
            _logger.LogDebug("Processing file [{File}]", file.Name);

            switch (file.Status)
            {
                case FileStatus.New:
                    await _mediatr.Send(new BackupFileCommand(file), cancellationToken);
                    break;
                case FileStatus.Updated:
                    await _mediatr.Send(new BackupFileCommand(file), cancellationToken);
                    break;
                case FileStatus.Unchanged:
                    await _mediatr.Send(new MoveFileFromPreviousBackupCommand(file), cancellationToken);
                    break;
            }
        }
    }
}
