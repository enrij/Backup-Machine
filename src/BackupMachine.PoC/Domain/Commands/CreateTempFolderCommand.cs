using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.PoC.Domain.Commands;

public class CreateTempFolderCommand : IRequest<DirectoryInfo>
{
}

public class CreateTempFolderHandler : RequestHandler<CreateTempFolderCommand, DirectoryInfo>
{
    private readonly ILogger<CreateTempFolderHandler> _logger;

    public CreateTempFolderHandler(ILogger<CreateTempFolderHandler> logger)
    {
        _logger = logger;
    }

    protected override DirectoryInfo Handle(CreateTempFolderCommand request)
    {
        var tempFolder = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "BackupMachine", Guid.NewGuid().ToString()));
        if (Directory.Exists(tempFolder.FullName) == false)
        {
            Directory.CreateDirectory(tempFolder.FullName);
        }

        _logger.LogDebug("Temporary worker folder [{Path}] created", tempFolder.FullName);

        return tempFolder;
    }
}
