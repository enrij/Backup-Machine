using BackupMachine.Core.Entities;
using BackupMachine.Core.Interfaces;

using MediatR;

namespace BackupMachine.Core.Commands;

public class CreateDestinationFolderStructureCommand : IRequest
{
    public CreateDestinationFolderStructureCommand(Backup backup)
    {
        Backup = backup;
    }

    public Backup Backup { get; set; }
}

public class CreateDestinationFolderStructureHandler : AsyncRequestHandler<CreateDestinationFolderStructureCommand>
{
    private readonly IFileSystemService _fileSystemService;
    private readonly IPersistenceService _persistenceService;

    public CreateDestinationFolderStructureHandler(IFileSystemService fileSystemService, IPersistenceService persistenceService)
    {
        _fileSystemService = fileSystemService;
        _persistenceService = persistenceService;
    }

    protected override async Task Handle(CreateDestinationFolderStructureCommand request, CancellationToken cancellationToken)
    {
        var foldersInBackup = await _persistenceService.GetAllFoldersForBackupAsync(request.Backup, cancellationToken);
        foreach (var folder in foldersInBackup)
        {
            _fileSystemService.CreateDirectory(Utilities.GetBackupFolderDestinationPath(folder));
        }
    }
}
