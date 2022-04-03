using BackupMachine.PoC.Domain.Commands;

using MediatR;

namespace BackupMachine.PoC.Domain.Handlers;

public class CreateTempFolderHandler : RequestHandler<CreateTempFolderCommand, DirectoryInfo>
{
    protected override DirectoryInfo Handle(CreateTempFolderCommand request)
    {
        var tempFolder = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "BackupMachine", Guid.NewGuid().ToString()));
        if (Directory.Exists(tempFolder.FullName) == false)
        {
            Directory.CreateDirectory(tempFolder.FullName);
        }

        return tempFolder;
    }
}
