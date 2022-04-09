using BackupMachine.Core.Entities;
using BackupMachine.Core.Services;
using BackupMachine.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BackupMachine.Console;

public class BackupHostedService : BackgroundService
{
    private readonly IServiceProvider _provider;

    public BackupHostedService(IDbContextFactory<BackupMachineContext> dbContextFactory, IServiceProvider provider)
    {
        _provider = provider;

        // TODO: Must find a different way to initialize database
        var context = dbContextFactory.CreateDbContext();
        context.Database.Migrate();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested == false)
        {
            using var scope = _provider.CreateScope();
            var jobsService = scope.ServiceProvider.GetRequiredService<JobsService>();
            var jobsList = await jobsService.GetJobsAsync(stoppingToken);
            
            foreach (var job in jobsList.TakeWhile(_=> stoppingToken.IsCancellationRequested == false))
            {
                var backupsService = scope.ServiceProvider.GetRequiredService<BackupsService>();
                await backupsService.ExecuteJobBackupAsync(job,stoppingToken);
            }
        }
    }
}
