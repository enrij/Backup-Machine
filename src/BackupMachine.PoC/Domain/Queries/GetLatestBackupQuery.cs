using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Domain.Queries;

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
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;

    public GetLatestBackupHandler(IDbContextFactory<BackupMachineContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Backup?> Handle(GetLatestBackupQuery request, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Backups
                            .Where(backup => backup.JobId == request.Job.Id)
                            .OrderByDescending(backup => backup.Timestamp)
                            .FirstOrDefaultAsync(cancellationToken);
    }
}
