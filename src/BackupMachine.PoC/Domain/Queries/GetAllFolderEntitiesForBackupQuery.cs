using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Domain.Queries;

public class GetAllFolderEntitiesForBackupQuery : IRequest<List<BackupFolder>>
{
    public GetAllFolderEntitiesForBackupQuery(Backup backup)
    {
        Backup = backup;
    }

    public Backup Backup { get; set; }
}

public class GetAllFolderEntitiesForBackupHandler : IRequestHandler<GetAllFolderEntitiesForBackupQuery, List<BackupFolder>>
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;

    public GetAllFolderEntitiesForBackupHandler(IDbContextFactory<BackupMachineContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<BackupFolder>> Handle(GetAllFolderEntitiesForBackupQuery request, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Folders
                            .Where(folder => folder.BackupId == request.Backup.Id)
                            .Include(folder => folder.Backup)
                            .ThenInclude(backup => backup.Job)
                            .ToListAsync(cancellationToken);
    }
}
