using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Domain.Commands;

public class CreateBackupCommand : IRequest<Backup>
{
    public CreateBackupCommand(Job job)
    {
        Job = job;
    }

    public Job Job { get; set; }
}

public class CreateBackupHandler : IRequestHandler<CreateBackupCommand, Backup>
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;

    public CreateBackupHandler(IDbContextFactory<BackupMachineContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Backup> Handle(CreateBackupCommand request, CancellationToken cancellationToken)
    {
        var backup = new Backup
        {
            Job = request.Job,
            Timestamp = DateTime.Now
        };
        
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        context.Entry(backup).State = EntityState.Added;
        await context.SaveChangesAsync(cancellationToken);

        return backup;
    }
}
