using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Domain.Queries;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Domain.Commands;

public class CreateBackupEntityCommand : IRequest<Backup>
{
    public CreateBackupEntityCommand(Job job)
    {
        Job = job;
    }

    public Job Job { get; init; }
}

public class CreateBackupEntityHandler : IRequestHandler<CreateBackupEntityCommand, Backup>
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;
    private readonly IMediator _mediatr;

    public CreateBackupEntityHandler(IDbContextFactory<BackupMachineContext> dbContextFactory, IMediator mediatr)
    {
        _dbContextFactory = dbContextFactory;
        _mediatr = mediatr;
    }

    public async Task<Backup> Handle(CreateBackupEntityCommand request, CancellationToken cancellationToken)
    {
        var backup = new Backup
        {
            Job = request.Job,
            Timestamp = DateTime.Now,
            PreviousBackup = await _mediatr.Send(new GetLatestBackupQuery(request.Job), cancellationToken)
        };

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        context.Entry(backup).State = EntityState.Added;
        await context.SaveChangesAsync(cancellationToken);

        return backup;
    }
}
