using MediatR;

namespace BackupMachine.PoC.Domain.Commands;

public class CopyFolderCommand : IRequest
{
    public CopyFolderCommand(DirectoryInfo source, DirectoryInfo destination, DateTime timestamp)
    {
        Source = source;
        Destination = destination;
        Timestamp = timestamp;
    }

    public DirectoryInfo Source { get; set; }
    public DirectoryInfo Destination { get; set; }
    public DateTime Timestamp { get; set; }
}
