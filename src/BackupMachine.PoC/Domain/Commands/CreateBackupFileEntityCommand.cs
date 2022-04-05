using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Domain.Queries;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Domain.Commands;

public class CreateBackupFileEntityCommand : IRequest<BackupFile>
{
    public CreateBackupFileEntityCommand(Backup backup, BackupFolder folder, FileInfo file)
    {
        Backup = backup;
        File = file;
        Folder = folder;
    }

    public Backup Backup { get; init; }
    public BackupFolder Folder { get; init; }
    public FileInfo File { get; init; }
}

public class CreateBackupFileEntityHandler : IRequestHandler<CreateBackupFileEntityCommand, BackupFile>
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;
    private readonly IMediator _mediatr;

    public CreateBackupFileEntityHandler(IDbContextFactory<BackupMachineContext> dbContextFactory, IMediator mediatr)
    {
        _dbContextFactory = dbContextFactory;
        _mediatr = mediatr;
    }

    public async Task<BackupFile> Handle(CreateBackupFileEntityCommand request, CancellationToken cancellationToken)
    {
        var folder = await _mediatr.Send(
            new GetBackupFolderEntityByPathQuery(
                Utilities.GetPathRelativeToTemporaryFolder(
                    request.File.DirectoryName ?? string.Empty,
                    request.Backup),
                request.Backup),
            cancellationToken);

        if (folder is null)
        {
            throw new InvalidOperationException("Folder not found");
        }

        var file = new BackupFile
        {
            Backup = request.Backup,
            Name = request.File.Name,
            Extension = request.File.Extension,
            BackupFolder = folder
        };

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        context.Entry(file).State = EntityState.Added;
        await context.SaveChangesAsync(cancellationToken);

        return file;
    }
}
