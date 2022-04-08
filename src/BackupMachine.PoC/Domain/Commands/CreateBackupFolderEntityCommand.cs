using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Domain.Queries;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Domain.Commands;

public class CreateBackupFolderEntityCommand : IRequest<BackupFolder>
{
    public CreateBackupFolderEntityCommand(Backup backup, DirectoryInfo sourceFolder)
    {
        Backup = backup;
        SourceFolder = sourceFolder;
    }

    public Backup Backup { get; init; }
    public DirectoryInfo SourceFolder { get; init; }
}

public class CreateBackupFolderEntityHandler : IRequestHandler<CreateBackupFolderEntityCommand, BackupFolder>
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;
    private readonly IMediator _mediatr;

    public CreateBackupFolderEntityHandler(IDbContextFactory<BackupMachineContext> dbContextFactory, IMediator mediatr)
    {
        _dbContextFactory = dbContextFactory;
        _mediatr = mediatr;
    }

    public async Task<BackupFolder> Handle(CreateBackupFolderEntityCommand request, CancellationToken cancellationToken)
    {
        var folder = new BackupFolder
        {
            Backup = request.Backup,
            RelativePath = Utilities.GetPathRelativeToJobSource(request.SourceFolder, request.Backup.Job),
            ParentFolder =
                request.SourceFolder.Parent is null
                    ? null
                    : await _mediatr.Send(new GetBackupFolderEntityByPathQuery(request.SourceFolder.Parent.FullName, request.Backup), cancellationToken)
        };

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        context.Entry(folder).State = EntityState.Added;
        await context.SaveChangesAsync(cancellationToken);

        return folder;
    }
}
