using BackupMachine.PoC.Domain.Entities;

using MediatR;

namespace BackupMachine.PoC.Domain.Commands;

public class StartJobCommand : IRequest
{
    public Job Job { get; set; }
    public StartJobCommand(Job job)
    {
        Job = job;
    }
}
