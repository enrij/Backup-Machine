using BackupMachine.Core.Entities;

namespace BackupMachine.Core;

public static class Utilities
{
    public static DirectoryInfo GetBackupDestinationRootFolderPath(Backup backup)
    {
        return new DirectoryInfo(Path.Combine(backup.Job.Destination, $"{backup.Timestamp:yyyy-MM-dd-HH-mm-ss}"));
    }

    public static string GetRelativePathFromJobSource(DirectoryInfo folder, Job job)
    {
        return folder.FullName.Replace(job.Source, string.Empty).TrimStart('\\');
    }

    public static DirectoryInfo GetBackupFolderDestinationPath(BackupFolder folder)
    {
        return new DirectoryInfo(Path.Combine(GetBackupDestinationRootFolderPath(folder.Backup).FullName, folder.RelativePath));
    }

    public static FileInfo GetBackupFileDestinationPath(BackupFile file)
    {
        return new FileInfo(Path.Combine(GetBackupDestinationRootFolderPath(file.Backup).FullName, file.BackupFolder.RelativePath, file.Name));
    }

    public static FileInfo GetBackupFileSourceInfo(BackupFile file)
    {
        return new FileInfo(Path.Combine(file.Backup.Job.Source, file.BackupFolder.RelativePath, file.Name));
    }

    public static FileInfo GetBackupFileDestinationPath(BackupFile file, Backup backup)
    {
        return new FileInfo(Path.Combine(GetBackupDestinationRootFolderPath(backup).FullName, file.BackupFolder.RelativePath, file.Name));
    }
}
