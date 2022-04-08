using BackupMachine.Core.Entities;
using BackupMachine.Core.Interfaces;
using BackupMachine.Core.Queries;

using MediatR;

namespace BackupMachine.Core.Commands;

public class CreateBackupEntityCommand : IRequest<Backup>
{
    public CreateBackupEntityCommand(Job job)
    {
        Job = job;
    }

    public Job Job { get; init; }
}

public class CreateBackupEntityHandler : IRequestHandler<CreateBackupEntityCommand, Backup>
{
    private readonly IPersistenceService _persistenceService;

    public CreateBackupEntityHandler(IPersistenceService persistenceService)
    {
        _persistenceService = persistenceService;
    }

    public async Task<Backup> Handle(CreateBackupEntityCommand request, CancellationToken cancellationToken)
    {
        var backup = new Backup
        {
            Job = request.Job,
            Timestamp = DateTime.Now,
            PreviousBackup = await _persistenceService.GetLatestBackupAsync(request.Job, cancellationToken)
        };

        backup = await _persistenceService.CreateBackupAsync(backup, cancellationToken);

        return backup;
    }
}
