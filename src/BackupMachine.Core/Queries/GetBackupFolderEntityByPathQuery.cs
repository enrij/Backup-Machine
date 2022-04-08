using BackupMachine.Core.Entities;
using BackupMachine.Core.Interfaces;

using MediatR;

namespace BackupMachine.Core.Queries;

public class GetBackupFolderEntityByPathQuery : IRequest<BackupFolder?>
{
    public GetBackupFolderEntityByPathQuery(string relativePath, Backup backup)
    {
        RelativePath = relativePath;
        Backup = backup;
    }

    public Backup Backup { get; init; }
    public string RelativePath { get; init; }
}

public class GetBackupFolderEntityByPathHandler : IRequestHandler<GetBackupFolderEntityByPathQuery, BackupFolder?>
{
    private readonly IPersistenceService _persistenceService;

    public GetBackupFolderEntityByPathHandler(IPersistenceService persistenceService)
    {
        _persistenceService = persistenceService;
    }

    public async Task<BackupFolder?> Handle(GetBackupFolderEntityByPathQuery request, CancellationToken cancellationToken)
    {
        return await _persistenceService.GetBackupFolderEntityByPath(request.RelativePath, request.Backup);
    }
}
