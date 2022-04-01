using BackupMachine.PoC.Domain;
using BackupMachine.PoC.Domain.Services;
using BackupMachine.PoC.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BackupMachine.PoC;

public class BackupHostedService : IHostedService
{
    private readonly BackupService _backupService;

    public BackupHostedService(IDbContextFactory<BackupMachineContext> dbContextFactory, BackupService backupService)
    {
        _backupService = backupService;

        var context = dbContextFactory.CreateDbContext();

        context.Database.Migrate();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _backupService.BackupAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
