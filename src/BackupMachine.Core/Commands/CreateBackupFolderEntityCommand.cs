using BackupMachine.Core.Entities;
using BackupMachine.Core.Interfaces;
using BackupMachine.Core.Queries;

using MediatR;

namespace BackupMachine.Core.Commands;

public class CreateBackupFolderEntityCommand : IRequest<BackupFolder>
{
    public CreateBackupFolderEntityCommand(Backup backup, DirectoryInfo sourceFolder)
    {
        Backup = backup;
        SourceFolder = sourceFolder;
    }

    public Backup Backup { get; init; }
    public DirectoryInfo SourceFolder { get; init; }
}

public class CreateBackupFolderEntityHandler : IRequestHandler<CreateBackupFolderEntityCommand, BackupFolder>
{
    private readonly IPersistenceService _persistenceService;

    public CreateBackupFolderEntityHandler(IPersistenceService persistenceService)
    {
        _persistenceService = persistenceService;
    }

    public async Task<BackupFolder> Handle(CreateBackupFolderEntityCommand request, CancellationToken cancellationToken)
    {
        var folder = new BackupFolder
        {
            Backup = request.Backup,
            RelativePath = Utilities.GetPathRelativeToJobSource(request.SourceFolder, request.Backup.Job),
            ParentFolder =
                request.SourceFolder.Parent is null
                    ? null
                    : await _persistenceService.GetBackupFolderByPathAsync(request.SourceFolder.Parent.FullName, request.Backup, cancellationToken)
        };

        folder = await _persistenceService.CreateBackupFolderAsync(folder, cancellationToken);

        return folder;
    }
}
