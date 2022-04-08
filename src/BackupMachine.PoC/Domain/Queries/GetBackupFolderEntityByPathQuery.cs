using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Domain.Queries;

public class GetBackupFolderEntityByPathQuery : IRequest<BackupFolder?>
{
    public GetBackupFolderEntityByPathQuery(string relativePath, Backup backup)
    {
        RelativePath = relativePath;
        Backup = backup;
    }

    public Backup Backup { get; init; }
    public string RelativePath { get; init; }
}

public class GetBackupFolderEntityByPathHandler : IRequestHandler<GetBackupFolderEntityByPathQuery, BackupFolder?>
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;

    public GetBackupFolderEntityByPathHandler(IDbContextFactory<BackupMachineContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<BackupFolder?> Handle(GetBackupFolderEntityByPathQuery request, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return context.Folders
                      .Where(folder => folder.BackupId == request.Backup.Id)
                      .Include(folder => folder.Files)
                      .AsEnumerable()
                      .FirstOrDefault(
                           folder => folder.RelativePath == request.RelativePath);
    }
}
