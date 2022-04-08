using BackupMachine.Core.Entities;
using BackupMachine.Core.Interfaces;
using BackupMachine.PoC.Domain.Aggregates;

using MediatR;

namespace BackupMachine.Core.Commands;

public class CreateBackupFileEntityCommand : IRequest<BackupFile>
{
    public CreateBackupFileEntityCommand(Backup backup, BackupFolder folder, FileToBackup file)
    {
        Backup = backup;
        File = file;
        Folder = folder;
    }

    public Backup Backup { get; init; }
    public BackupFolder Folder { get; init; }
    public FileToBackup File { get; init; }
}

public class CreateBackupFileEntityHandler : IRequestHandler<CreateBackupFileEntityCommand, BackupFile>
{
    private readonly IPersistenceService _persistenceService;

    public CreateBackupFileEntityHandler(IPersistenceService persistenceService)
    {
        _persistenceService = persistenceService;
    }

    public async Task<BackupFile> Handle(CreateBackupFileEntityCommand request, CancellationToken cancellationToken)
    {
        var file = new BackupFile
        {
            Backup = request.Backup,
            Name = request.File.FileInfo.Name,
            Extension = request.File.FileInfo.Extension,
            BackupFolder = request.Folder,
            Status = request.File.Status,
            Length = request.File.FileInfo.Length,
            Modified = request.File.FileInfo.LastWriteTimeUtc,
            Created = request.File.FileInfo.CreationTimeUtc
        };

        file = await _persistenceService.CreateBackupFile(file, cancellationToken);

        return file;
    }
}
