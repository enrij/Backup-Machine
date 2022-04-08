using BackupMachine.Core.Entities;
using BackupMachine.Core.Interfaces;

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
    private readonly IFileSystemService _fileSystemService;
    private readonly ILogger<BackupFileHandler> _logger;

    public BackupFileHandler(ILogger<BackupFileHandler> logger, IFileSystemService fileSystemService)
    {
        _logger = logger;
        _fileSystemService = fileSystemService;
    }

    protected override void Handle(BackupFileCommand request)
    {
        var file = Utilities.GetBackupFileSourceInfo(request.File);
        var destination = Utilities.GetBackupFileDestinationPath(request.File);

        if (!file.Exists)
        {
            _logger.LogWarning("File [{File}] not found", file.FullName);
            return;
        }

        _fileSystemService.CopyFile(file, destination);

        _logger.LogDebug("File [{File}] copied", file.FullName);
    }
}
