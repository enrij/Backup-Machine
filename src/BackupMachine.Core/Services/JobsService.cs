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

    public async Task<List<Job>> GetJobs()
    {
        return await _persistenceService.GetJobsAsync();
    }
}
