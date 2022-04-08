using BackupMachine.Core.Commands;
using BackupMachine.Infrastructure.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BackupMachine.Console;

public class BackupHostedService : IHostedService
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;
    private readonly IServiceProvider _serviceProvider;

    public BackupHostedService(IDbContextFactory<BackupMachineContext> dbContextFactory, IServiceProvider serviceProvider)
    {
        _dbContextFactory = dbContextFactory;
        _serviceProvider = serviceProvider;
        var context = dbContextFactory.CreateDbContext();

        context.Database.Migrate();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var job in context.Jobs)
        {
            await mediatr.Send(new ExecuteJobCommand(job), cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
