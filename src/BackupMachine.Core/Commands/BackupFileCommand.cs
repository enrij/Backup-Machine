using BackupMachine.Core.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.Core.Commands;

public class BackupFileCommand : IRequest
{
    public BackupFileCommand(BackupFile file)
    {
        File = file;
    }

    public BackupFile File { get; set; }
}

public class BackupFileHandler : RequestHandler<BackupFileCommand>
{
    private readonly ILogger<BackupFileHandler> _logger;

    public BackupFileHandler(ILogger<BackupFileHandler> logger)
    {
        _logger = logger;
    }

    protected override void Handle(BackupFileCommand request)
    {
        var file = new FileInfo(Path.Combine(request.File.Backup.Job.Source, request.File.BackupFolder.RelativePath, request.File.Name));
        var destination = new DirectoryInfo(Utilities.GetBackupFileDestinationPath(request.File));

        if (!file.Exists)
        {
            _logger.LogWarning("File [{File}] not found", file.FullName);
            return;
        }

        file.CopyTo(destination.FullName, true);

        _logger.LogDebug("File [{File}] copied", file.FullName);
    }
}
