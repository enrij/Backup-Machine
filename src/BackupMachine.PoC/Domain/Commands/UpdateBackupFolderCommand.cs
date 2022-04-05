using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Domain.Commands;

public class UpdateBackupFolderCommand : IRequest<BackupFolder>
{
    public UpdateBackupFolderCommand(BackupFolder entity)
    {
        Entity = entity;
    }

    public BackupFolder Entity { get; set; }
}

public class UpdateBackupFolderHandler : IRequestHandler<UpdateBackupFolderCommand, BackupFolder>
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;

    public UpdateBackupFolderHandler(IDbContextFactory<BackupMachineContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<BackupFolder> Handle(UpdateBackupFolderCommand request, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var folder = request.Entity;
        await context.SaveChangesAsync(cancellationToken);

        return folder;
    }
}
