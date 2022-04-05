/*
using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Domain.Queries;

public class GetLatestBackupFolderEntityFromPathQuery : IRequest<BackupFolder?>
{
    public GetLatestBackupFolderEntityFromPathQuery(string relativePath)
    {
        RelativePath = relativePath;
    }

    public string RelativePath { get; set; }
}

public class GetBackupFolderEntitiesBySourcePathHandler : IRequestHandler<GetLatestBackupFolderEntityFromPathQuery, BackupFolder?>
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;

    public GetBackupFolderEntitiesBySourcePathHandler(IDbContextFactory<BackupMachineContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<BackupFolder?> Handle(GetLatestBackupFolderEntityFromPathQuery request, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return context.Folders
                      .Include(folder => folder.Files)
                      .AsEnumerable()
                      .FirstOrDefault(folder => folder.RelativePath == request.RelativePath);
    }
}
*/


