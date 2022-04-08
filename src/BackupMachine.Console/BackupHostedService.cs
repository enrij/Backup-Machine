using BackupMachine.Core.Services;
using BackupMachine.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BackupMachine.Console;

public class BackupHostedService : IHostedService
{
    private readonly BackupsService _backupsService;
    private readonly JobsService _jobsService;

    public BackupHostedService(IDbContextFactory<BackupMachineContext> dbContextFactory, BackupsService backupsService, JobsService jobsService)
    {
        _backupsService = backupsService;
        _jobsService = jobsService;

        // TODO: Must find a different way to initialize database
        var context = dbContextFactory.CreateDbContext();
        context.Database.Migrate();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var jobsList = await _jobsService.GetJobs();

        foreach (var job in jobsList)
        {
            await _backupsService.ExecuteJobBackupAsync(job, cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
