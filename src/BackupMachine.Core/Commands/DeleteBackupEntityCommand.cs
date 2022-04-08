using BackupMachine.Core.Entities;
using BackupMachine.Core.Interfaces;

using MediatR;

namespace BackupMachine.Core.Commands;

public class DeleteBackupEntityCommand : IRequest
{
    public DeleteBackupEntityCommand(Backup backup)
    {
        Backup = backup;
    }

    public Backup Backup { get; set; }
}

public class DeleteBackupEntityHandler : AsyncRequestHandler<DeleteBackupEntityCommand>
{
    private readonly IPersistenceService _persistenceService;

    public DeleteBackupEntityHandler(IPersistenceService persistenceService)
    {
        _persistenceService = persistenceService;
    }

    protected override async Task Handle(DeleteBackupEntityCommand request, CancellationToken cancellationToken)
    {
        await _persistenceService.DeleteBackupEntity(request.Backup, cancellationToken);
    }
}
