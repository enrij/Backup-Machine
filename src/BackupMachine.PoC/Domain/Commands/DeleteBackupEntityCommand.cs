using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Domain.Commands;

public class DeleteBackupEntityCommand : IRequest
{
    public DeleteBackupEntityCommand(Backup backup)
    {
        Backup = backup;
    }

    public Backup Backup { get; set; }
}

public class DeleteBackupEntityHandler : AsyncRequestHandler<DeleteBackupEntityCommand>
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;

    public DeleteBackupEntityHandler(IDbContextFactory<BackupMachineContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task Handle(DeleteBackupEntityCommand request, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var filesToDelete = context.Files
                                   .Where(file => file.BackupId == request.Backup.Id)
                                   .ToList();
        
        var foldersToDelete  = context.Folders
                                   .Where(file => file.BackupId == request.Backup.Id)
                                   .ToList();
        
        context.RemoveRange(filesToDelete);
        context.RemoveRange(foldersToDelete);
        context.Remove(request.Backup);

        await context.SaveChangesAsync(cancellationToken);
    }
}
