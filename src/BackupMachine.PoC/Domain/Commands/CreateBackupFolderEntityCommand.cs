using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Domain.Queries;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Domain.Commands;

public class CreateBackupFolderEntityCommand : IRequest<BackupFolder>
{
    public CreateBackupFolderEntityCommand(Backup backup, DirectoryInfo source, DirectoryInfo destination)
    {
        Backup = backup;
        Source = source;
        Destination = destination;
    }

    public Backup Backup { get; set; }
    public DirectoryInfo Source { get; set; }
    public DirectoryInfo Destination { get; set; }
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
            Source = request.Source,
            Destination = request.Destination,
            ParentFolder = 
                request.Destination.Parent is null
                ? null
                : await _mediatr.Send(new GetBackupFolderEntityByPathQuery(request.Destination.Parent.FullName, request.Backup.Id), cancellationToken)
        };

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        context.Entry(folder).State = EntityState.Added;
        await context.SaveChangesAsync(cancellationToken);

        return folder;
    }
}
