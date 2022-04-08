using BackupMachine.Core.Interfaces;

namespace BackupMachine.Infrastructure.FileSystem;

public class FileSystemService : IFileSystemService
{
    public void CopyFile(FileInfo file, FileInfo destination, bool overwrite = false)
    {
        file.CopyTo(destination.FullName, overwrite);
    }

    public void DeleteFolder(DirectoryInfo directory, bool recursive = false)
    {
        if (directory.Exists == false)
        {
            return;
        }

        foreach (var file in directory.GetFiles())
        {
            file.Delete();
        }

        if (recursive)
        {
            foreach (var subfolder in directory.GetDirectories())
            {
                DeleteFolder(subfolder, recursive);
            }
        }

        directory.Delete();
    }

    public void CreateDirectory(DirectoryInfo directory)
    {
        if (directory.Exists == false)
        {
            directory.Create();
        }
    }

    public void MoveFile(FileInfo source, FileInfo destination, bool overwrite = false)
    {
        source.MoveTo(destination.FullName, overwrite);
    }
}
