using MediatR;

namespace BackupMachine.Core.Commands;

public class DeleteFolderCommand : IRequest
{
    public DeleteFolderCommand(string path)
    {
        Path = path;
    }

    public string Path { get; set; }
}

public class DeleteFolderHandler : AsyncRequestHandler<DeleteFolderCommand>
{
    private readonly IMediator _mediator;

    public DeleteFolderHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    protected override async Task Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
    {
        if (Directory.Exists(request.Path) == false)
        {
            return;
        }

        var folder = new DirectoryInfo(request.Path);

        foreach (var file in folder.GetFiles())
        {
            file.Delete();
        }

        foreach (var subfolder in folder.GetDirectories())
        {
            await _mediator.Send(new DeleteFolderCommand(subfolder.FullName), cancellationToken);
        }

        folder.Delete();
    }
}
