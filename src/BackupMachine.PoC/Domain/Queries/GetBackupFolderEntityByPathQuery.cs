using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Domain.Queries;

public class GetBackupFolderEntityByPathQuery : IRequest<BackupFolder>
{
    public GetBackupFolderEntityByPathQuery(string path, Guid backupId)
    {
        Path = path;
        BackupId = backupId;
    }

    public Guid BackupId { get; set; }
    public string Path { get; set; }
}

public class GetBackupFolderEntityByPathHandler : IRequestHandler<GetBackupFolderEntityByPathQuery, BackupFolder>
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;

    public GetBackupFolderEntityByPathHandler(IDbContextFactory<BackupMachineContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<BackupFolder> Handle(GetBackupFolderEntityByPathQuery request, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return context.Folders
                      .Where(folder => folder.BackupId == request.BackupId)
                      .AsEnumerable()
                      .First(
                          folder => folder.Destination.FullName == request.Path);
    }
}
