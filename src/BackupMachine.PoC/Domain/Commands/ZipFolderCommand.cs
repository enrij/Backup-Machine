using System.IO.Compression;

using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.PoC.Domain.Commands;

public class ZipFolderCommand : IRequest
{
    public ZipFolderCommand(DirectoryInfo folder, DateTime timeStamp)
    {
        Folder = folder;
        TimeStamp = timeStamp;
    }

    public DateTime TimeStamp { get; init; }

    public DirectoryInfo Folder { get; init; }
}

public class ZipFolderHandler : AsyncRequestHandler<ZipFolderCommand>
{
    private readonly ILogger<ZipFolderHandler> _logger;
    private readonly IMediator _mediator;

    public ZipFolderHandler(IMediator mediator, ILogger<ZipFolderHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override async Task Handle(ZipFolderCommand request, CancellationToken cancellationToken)
    {
        /*
         * Zip archive cannot be created in the same folder you are zipping so it will be created on parent folder and then moved to correct folder.
         * Because of this we need to create the archives for subfolder BEFORE the archive for current folder otherwise (since all the archives have
         * the same file name) the archive created for current folder will block the creation of the archive for child folders (since they have will be
         * temporary stored in their parent folder)
         */
        foreach (var directory in request.Folder.GetDirectories().TakeWhile(_ => !cancellationToken.IsCancellationRequested))
        {
            await _mediator.Send(new ZipFolderCommand(directory, request.TimeStamp), cancellationToken);
        }

        var archiveFile = new FileInfo(Path.Combine(request.Folder.Parent!.FullName, $"BackupMachine-{request.TimeStamp:yyyy MM dd HH mm ss}.zip"));
        var filesToZip = request.Folder.GetFiles().Where(file => file.Name != archiveFile.Name && file.Attributes != FileAttributes.Hidden).ToList();

        if (filesToZip.Count > 0)
        {
            _logger.LogDebug("Zipping folder {Folder}", request.Folder.FullName);

            var archive = new ZipArchive(archiveFile.Create(), ZipArchiveMode.Create);

            foreach (var file in filesToZip.TakeWhile(_ => !cancellationToken.IsCancellationRequested))
            {
                _logger.LogDebug("Adding file {File} to archive", file.FullName);
                archive.CreateEntryFromFile(file.FullName, file.Name);
            }

            archive.Dispose();

            _logger.LogDebug("Zip archive created on parent directory");

            archiveFile.MoveTo(Path.Combine(request.Folder.FullName, archiveFile.Name));

            _logger.LogDebug("Zip archive moved into position");
            _logger.LogDebug("Zipped folder {Folder}", request.Folder.FullName);
        }
    }
}
