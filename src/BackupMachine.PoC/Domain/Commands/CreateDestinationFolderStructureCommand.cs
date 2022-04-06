using BackupMachine.PoC.Domain.Entities;
using BackupMachine.PoC.Domain.Queries;

using MediatR;

namespace BackupMachine.PoC.Domain.Commands;

public class CreateDestinationFolderStructureCommand : IRequest
{
    public CreateDestinationFolderStructureCommand(Backup backup)
    {
        Backup = backup;
    }

    public Backup Backup { get; set; }
}

public class CreateDestinationFolderStructureHandler : AsyncRequestHandler<CreateDestinationFolderStructureCommand>
{
    private readonly IMediator _mediatr;

    public CreateDestinationFolderStructureHandler(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    protected override async Task Handle(CreateDestinationFolderStructureCommand request, CancellationToken cancellationToken)
    {
        var foldersInBackup = await _mediatr.Send(new GetAllFolderEntitiesForBackupQuery(request.Backup), cancellationToken);
        foreach (var folder in foldersInBackup)
        {
            Directory.CreateDirectory(Utilities.GetBackupFolderDestinationPath(folder));
        }
    }
}
