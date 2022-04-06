using BackupMachine.PoC.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.PoC.Domain.Commands;

public class BackupNewFileCommand : IRequest
{
    public BackupNewFileCommand(BackupFile file)
    {
        File = file;
    }

    public BackupFile File { get; set; }
}

public class BackupNewFileHandler : RequestHandler<BackupNewFileCommand>
{
    private readonly ILogger<BackupNewFileHandler> _logger;

    public BackupNewFileHandler(ILogger<BackupNewFileHandler> logger)
    {
        _logger = logger;
    }

    protected override void Handle(BackupNewFileCommand request)
    {
        var source = new FileInfo(Path.Combine(request.File.Backup.Job.Source, request.File.BackupFolder.RelativePath, request.File.Name));
        var destination = new DirectoryInfo(Utilities.GetBackupFileDestinationPath(request.File));

        _logger.LogDebug("Copying file [{File}]", source.FullName);

        if (!source.Exists)
        {
            _logger.LogWarning("File [{File}] not found]", source.FullName);
        }

        source.CopyTo(destination.FullName);
    }
}
