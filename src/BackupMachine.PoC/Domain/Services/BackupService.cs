using BackupMachine.PoC.Infrastructure;
using BackupMachine.PoC.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Domain.Services;

public class BackupService
{
    private readonly IDbContextFactory<BackupMachineContext> _dbContextFactory;

    public BackupService(IDbContextFactory<BackupMachineContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public DirectoryInfo ComputeDestinationInfo(string path)
    {
        return new DirectoryInfo(Path.Combine(path, $"{Path.GetDirectoryName(path)}_{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}"));
    }

    public async Task<FileCopy> BackupFileAsync(FileInfo source, FileInfo destination, CancellationToken cancellationToken = default)
    {
        File.Copy(source.FullName, destination.FullName, true);

        Console.WriteLine($"Copied file {source.FullName} into {destination.FullName}");

        return await Task.FromResult(new FileCopy
        {
            Name = source.Name,
            Extension = source.Extension,
            OriginalPath = source.FullName
        });
    }

    public async Task<FolderCopy> BackupDirectoryAsync(DirectoryInfo source, DirectoryInfo destination, Snapshot snapshot, FolderCopy? parent, CancellationToken cancellationToken = default)
    {
        var folder = new FolderCopy
        {
            Name = destination.Name,
            Snapshot = snapshot,
            ParentFolder = parent
        };

        if (!destination.Exists)
        {
            destination.Create();
        }

        foreach (var file in source.GetFiles())
        {
            var fileInfo = new FileInfo(Path.Combine(destination.FullName, file.Name));
            var fileCopy = await BackupFileAsync(file, fileInfo, cancellationToken);
            fileCopy.Folder = folder;
            fileCopy.Snapshot = snapshot;
            folder.Files.Add(fileCopy);
        }

        foreach (var directory in source.GetDirectories())
        {
            var folderCopy = await BackupDirectoryAsync(directory, new DirectoryInfo(Path.Combine(destination.FullName, directory.Name)), snapshot, parent, cancellationToken);
            
            folder.Folders.Add(folderCopy);
        }

        return folder;
    }

    public async Task BackupAsync(CancellationToken cancellationToken = default)
    {
        var snapshot = new Snapshot
        {
            Timestamp = DateTime.Now
        };

        var pairs = (
                from source in TaskSettings.Sources.Where(Directory.Exists)
                from destination in TaskSettings.Destinations
                select new SourceDestinationPair(source, destination)
            )
            .ToList();

        var folders = new List<FolderCopy>();
        
        foreach (var pair in pairs)
        {
            var sourceInfo = new DirectoryInfo(pair.Source);
            var destinationInfo = ComputeDestinationInfo(pair.Destination);

            var folder = new FolderCopy
            {
                Name = sourceInfo.Name,
                Snapshot = snapshot,
                OriginalPath = sourceInfo.FullName
            };
                
            if (Directory.Exists(destinationInfo.FullName) == false)
            {
                Directory.CreateDirectory(destinationInfo.FullName);
            }

            foreach (var file in sourceInfo.GetFiles())
            {
                var fileInfo = new FileInfo(Path.Combine(destinationInfo.FullName, file.Name));
                var fileCopy = await BackupFileAsync(file, fileInfo, cancellationToken);
                fileCopy.Folder = folder;
                fileCopy.Snapshot = snapshot;
                folder.Files.Add(fileCopy);
            }

            foreach (var directory in sourceInfo.GetDirectories())
            {
                var destinationDirectory = new DirectoryInfo(Path.Combine(destinationInfo.FullName, directory.Name));
                var folderCopy = await BackupDirectoryAsync(directory, destinationDirectory, snapshot, folder, cancellationToken);
                folder.Folders.Add(folderCopy);
            }
            
            folders.Add(folder);
        }
            
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await db.Folders.AddRangeAsync(folders, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}
