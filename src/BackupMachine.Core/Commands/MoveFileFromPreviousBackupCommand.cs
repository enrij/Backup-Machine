using BackupMachine.Core.Entities;
using BackupMachine.Core.Interfaces;

using MediatR;

namespace BackupMachine.Core.Commands;

public class MoveFileFromPreviousBackupCommand : IRequest
{
    public MoveFileFromPreviousBackupCommand(BackupFile file)
    {
        File = file;
    }

    public BackupFile File { get; set; }
}

public class MoveFileFromPreviousBackupHandler : RequestHandler<MoveFileFromPreviousBackupCommand>
{
    private readonly IFileSystemService _fileSystemService;

    public MoveFileFromPreviousBackupHandler(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;
    }

    protected override void Handle(MoveFileFromPreviousBackupCommand request)
    {
        if (request.File.Backup.PreviousBackup is null)
        {
            throw new InvalidDataException("File has no previous backup");
        }

        var source = Utilities.GetBackupFileDestinationPath(request.File, request.File.Backup.PreviousBackup);
        var destination = Utilities.GetBackupFileDestinationPath(request.File);

        _fileSystemService.MoveFile(source, destination);
    }
}
