using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Domain.Queries;

public class GetAllFileEntitiesForBackupQuery : IRequest<List<BackupFile>>
{
    public GetAllFileEntitiesForBackupQuery(Backup backup)
    {
        Backup = backup;
    }

    public Backup Backup { get; set; }
}

public class GetAllFileEntitiesForBackupHandler : IRequestHandler<GetAllFileEntitiesForBackupQuery, List<BackupFile>>
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;

    public GetAllFileEntitiesForBackupHandler(IDbContextFactory<BackupMachineContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<BackupFile>> Handle(GetAllFileEntitiesForBackupQuery request, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Files
                            .Where(file => file.BackupId == request.Backup.Id)
                            .Include(file => file.Backup)
                            .ThenInclude(backup => backup.Job)
                            .Include(file => file.Backup)
                            .ThenInclude(backup => backup.PreviousBackup)
                            .Include(file => file.BackupFolder)
                            .ToListAsync(cancellationToken);
    }
}
