using BackupMachine.PoC.Domain.Commands;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BackupMachine.PoC;

public class BackupHostedService : IHostedService
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;
    private readonly IMediator _mediatr;

    public BackupHostedService(IDbContextFactory<BackupMachineContext> dbContextFactory, IMediator mediatr)
    {
        _dbContextFactory = dbContextFactory;
        _mediatr = mediatr;
        var context = dbContextFactory.CreateDbContext();

        context.Database.Migrate();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var job in context.Jobs)
        {
            await _mediatr.Send(new ExecuteJobCommand(job), cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
