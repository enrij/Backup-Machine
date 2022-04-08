using BackupMachine.Core.Entities;
using BackupMachine.Core.Interfaces;

using MediatR;

namespace BackupMachine.Core.Queries;

public class GetLatestBackupQuery : IRequest<Backup>
{
    public GetLatestBackupQuery(Job job)
    {
        Job = job;
    }

    public Job Job { get; init; }
}

public class GetLatestBackupHandler : IRequestHandler<GetLatestBackupQuery, Backup?>
{
    private readonly IPersistenceService _persistenceService;

    public GetLatestBackupHandler(IPersistenceService persistenceService)
    {
        _persistenceService = persistenceService;
    }

    public async Task<Backup?> Handle(GetLatestBackupQuery request, CancellationToken cancellationToken)
    {
        return await _persistenceService.GetLatestBackupAsync(request.Job, cancellationToken);
    }
}
