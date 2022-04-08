using BackupMachine.Core.Entities;
using BackupMachine.Core.Interfaces;

using MediatR;

namespace BackupMachine.Core.Queries;

public class GetAllFileEntitiesForBackupQuery : IRequest<List<BackupFile>>
{
    public GetAllFileEntitiesForBackupQuery(Backup backup)
    {
        Backup = backup;
    }

    public Backup Backup { get; set; }
}

public class GetAllFileEntitiesForBackupHandler : IRequestHandler<GetAllFileEntitiesForBackupQuery, List<BackupFile>>
{
    private readonly IPersistenceService _persistenceService;

    public GetAllFileEntitiesForBackupHandler(IPersistenceService persistenceService)
    {
        _persistenceService = persistenceService;
    }

    public async Task<List<BackupFile>> Handle(GetAllFileEntitiesForBackupQuery request, CancellationToken cancellationToken)
    {
        return await _persistenceService.GetAllFilesForBackupAsync(request.Backup, cancellationToken);
    }
}
