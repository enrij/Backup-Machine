using BackupMachine.Core.Entities;
using BackupMachine.Core.Interfaces;

namespace BackupMachine.Core.Services;

public class JobsService
{
    private readonly IPersistenceService _persistenceService;

    public JobsService(IPersistenceService persistenceService)
    {
        _persistenceService = persistenceService;
    }

    public async Task<List<Job>> GetJobsAsync(CancellationToken cancellationToken = default)
    {
        return await _persistenceService.GetJobsAsync(cancellationToken);
    }
}
