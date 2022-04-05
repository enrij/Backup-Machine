using BackupMachine.PoC.Domain.Entities;

using MediatR;

namespace BackupMachine.PoC.Domain.Commands;

public class CreateDatabaseDataFromFolderCommand : IRequest<BackupFolder>
{
    public CreateDatabaseDataFromFolderCommand(Backup backup, DirectoryInfo folder)
    {
        Backup = backup;
        Folder = folder;
    }

    public DirectoryInfo Folder { get; init; }
    public Backup Backup { get; init; }
}

public class CreateDatabaseDataFromFolderHandler : IRequestHandler<CreateDatabaseDataFromFolderCommand, BackupFolder>
{
    private readonly IMediator _mediatr;

    public CreateDatabaseDataFromFolderHandler(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task<BackupFolder> Handle(CreateDatabaseDataFromFolderCommand request, CancellationToken cancellationToken)
    {
        
        var relativePath = request.Folder.FullName.Replace($"{Utilities.ComposeTemporaryFolderPath(request.Backup)}\\", string.Empty);
        
        var source = new DirectoryInfo(Path.Combine(request.Backup.Job.Source, relativePath));
        var destination = new DirectoryInfo(Path.Combine(request.Backup.Job.Destination, relativePath));
        
        var destinationEntity = await _mediatr.Send(new CreateBackupFolderEntityCommand(request.Backup, source, destination), cancellationToken);

        foreach (var file in request.Folder.GetFiles().TakeWhile(_ => cancellationToken.IsCancellationRequested == false))
        {
            var fileEntity = await _mediatr.Send(new CreateBackupFileEntityCommand(request.Backup, destinationEntity, file), cancellationToken);
            destinationEntity.Files.Add(fileEntity);
        }

        foreach (var directory in request.Folder.GetDirectories().TakeWhile(_ => cancellationToken.IsCancellationRequested == false))
        {
            var subfolderEntity = await _mediatr.Send(new CreateDatabaseDataFromFolderCommand(request.Backup, directory), cancellationToken);
            destinationEntity.Subfolders.Add(subfolderEntity);
        }
        
        if (destinationEntity.Files.Count > 0 || destinationEntity.Subfolders.Count > 0)
        {
            destinationEntity = await _mediatr.Send(new UpdateBackupFolderCommand(destinationEntity), cancellationToken);
        }

        return destinationEntity;
    }
}
