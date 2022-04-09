namespace BackupMachine.Core.Interfaces;

public interface IFileSystemService
{
    void CopyFile(FileInfo file, FileInfo destination, bool overwrite = false);
    void DeleteFolderAndContent(DirectoryInfo directory, bool recursive = false);
    void CreateDirectory(DirectoryInfo directory);
    void MoveFile(FileInfo source, FileInfo destination, bool overwrite = false);
}
