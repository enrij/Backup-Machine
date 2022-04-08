using BackupMachine.Core.Entities;
using BackupMachine.Core.Interfaces;

using MediatR;

namespace BackupMachine.Core.Commands;

public class UpdateBackupFolderCommand : IRequest<BackupFolder>
{
    public UpdateBackupFolderCommand(BackupFolder folder)
    {
        Folder = folder;
    }

    public BackupFolder Folder { get; init; }
}

public class UpdateBackupFolderHandler : IRequestHandler<UpdateBackupFolderCommand, BackupFolder>
{
    private readonly IPersistenceService _persistenceService;

    public UpdateBackupFolderHandler(IPersistenceService persistenceService)
    {
        _persistenceService = persistenceService;
    }

    public async Task<BackupFolder> Handle(UpdateBackupFolderCommand request, CancellationToken cancellationToken)
    {
        return await _persistenceService.UpdateBackupFolderAsync(request.Folder, cancellationToken);
    }
}
