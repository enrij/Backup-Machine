using BackupMachine.PoC.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.PoC.Domain.Commands;

public class MoveBackupFileToPreviousVersionCommand : IRequest
{
    public MoveBackupFileToPreviousVersionCommand(BackupFile file)
    {
        File = file;
    }

    public BackupFile File { get; set; }
}

public class MoveBackupFileToPreviousVersionHandler : RequestHandler<MoveBackupFileToPreviousVersionCommand>
{
    private readonly ILogger<MoveBackupFileToPreviousVersionHandler> _logger;

    public MoveBackupFileToPreviousVersionHandler(ILogger<MoveBackupFileToPreviousVersionHandler> logger)
    {
        _logger = logger;
    }

    protected override void Handle(MoveBackupFileToPreviousVersionCommand request)
    {
        var file = new FileInfo(Path.Combine(request.File.Backup.Job.Destination, request.File.BackupFolder.RelativePath, request.File.Name));
        var destination = new DirectoryInfo(Utilities.GetBackupFileDestinationPath(request.File));

        _logger.LogDebug("Moving file [{File}] to previous backup", file.FullName);

        if (!file.Exists)
        {
            _logger.LogWarning("File [{File}] not found", file.FullName);
        }

        file.Delete();
    }
}
