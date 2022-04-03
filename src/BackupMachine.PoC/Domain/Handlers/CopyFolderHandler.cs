using System.IO.Compression;

using BackupMachine.PoC.Domain.Commands;

using MediatR;

using Microsoft.Extensions.Logging;

namespace BackupMachine.PoC.Domain.Handlers;

public class CopyFolderHandler : AsyncRequestHandler<CopyFolderCommand>
{
    private readonly IMediator _mediatr;
    private readonly ILogger<CopyFolderHandler> _logger;

    public CopyFolderHandler(IMediator mediatr, ILogger<CopyFolderHandler> logger)
    {
        _mediatr = mediatr;
        _logger = logger;
    }

    protected override async Task Handle(CopyFolderCommand request, CancellationToken cancellationToken)
    {
        foreach (var file in request.Source.GetFiles())
        {
            _logger.LogDebug("Copying file {File}", file.FullName);
            File.Copy(file.FullName, Path.Combine(request.Destination.FullName, $"{file.Name}.{file.Extension}"));
            _logger.LogDebug("Copied file {File}", file.FullName);
        }

        if (request.Source.GetFiles().Length > 0)
        {
            _logger.LogDebug("Creating zip archive");

            // Create the archive into parent folder because you cannot create a file in the folder you are zipping
            var archive = new FileInfo(Path.Combine(request.Destination.Parent!.FullName, $"{request.Timestamp:yyyy-MM-dd-HH-mm-ss}.zip"));
            
            ZipFile.CreateFromDirectory(request.Destination.FullName, archive.FullName);

            _logger.LogDebug("Zip archive created");
            
            // Move the archive to proper location
            // TODO: "Proper location" should be into request.Destination folder/subfolder
            File.Move(archive.FullName, Path.Combine(request.Destination.FullName, $"{archive.Name}.{archive.Extension}"));
        }

        foreach (var directory in request.Source.GetDirectories())
        {
            var destination = Directory.CreateDirectory(Path.Combine(request.Destination.FullName, directory.Name));
            _logger.LogDebug("Copying folder {Folder}", directory.FullName);
            await _mediatr.Send(new CopyFolderCommand(directory, destination, request.Timestamp), cancellationToken);
            _logger.LogDebug("Copied folder {Folder}", directory.FullName);
        }
        
        // TODO: Cleanup
    }
}
