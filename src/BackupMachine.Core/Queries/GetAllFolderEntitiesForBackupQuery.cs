using BackupMachine.Core.Entities;
using BackupMachine.Core.Interfaces;

using MediatR;

namespace BackupMachine.Core.Queries;

public class GetAllFolderEntitiesForBackupQuery : IRequest<List<BackupFolder>>
{
    public GetAllFolderEntitiesForBackupQuery(Backup backup)
    {
        Backup = backup;
    }

    public Backup Backup { get; set; }
}

public class GetAllFolderEntitiesForBackupHandler : IRequestHandler<GetAllFolderEntitiesForBackupQuery, List<BackupFolder>>
{
    private readonly IPersistenceService _persistenceService;

    public GetAllFolderEntitiesForBackupHandler(IPersistenceService persistenceService)
    {
        _persistenceService = persistenceService;
    }

    public async Task<List<BackupFolder>> Handle(GetAllFolderEntitiesForBackupQuery request, CancellationToken cancellationToken)
    {
        return await _persistenceService.GetAllFoldersForBackupAsync(request.Backup, cancellationToken);
    }
}
