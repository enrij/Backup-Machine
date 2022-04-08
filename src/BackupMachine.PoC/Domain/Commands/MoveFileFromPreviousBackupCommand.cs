using BackupMachine.PoC.Domain.Entities;

using MediatR;

namespace BackupMachine.PoC.Domain.Commands;

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
    protected override void Handle(MoveFileFromPreviousBackupCommand request)
    {
        if (request.File.Backup.PreviousBackup is null)
        {
            throw new InvalidDataException("File has no previous backup");
        }

        var source = new FileInfo(Utilities.GetBackupFileDestinationPath(request.File, request.File.Backup.PreviousBackup));
        var destination = new FileInfo(Utilities.GetBackupFileDestinationPath(request.File));
        
        source.MoveTo(destination.FullName);
    }
}
