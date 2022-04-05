using BackupMachine.PoC.Domain.Entities;

namespace BackupMachine.PoC.Domain;

public static class Utilities
{
    public static string ComposeBackupArchiveName(Backup backup)
    {
        return $"BackupMachine-{backup.Timestamp:yyyy MM dd HH mm ss}.zip";
    }

    public static string ComposeTemporaryFolderPath(Backup backup)
    {
        return Path.Combine(Path.GetTempPath(), "BackupMachine", backup.Id.ToString());
    }

    public static string GetPathRelativeToJobSource(string path, Job job)
    {
        return path.Replace(job.Source, string.Empty).TrimStart('\\');
    }

    public static string GetPathRelativeToJobDestination(string path, Job job)
    {
        return path.Replace(job.Destination, string.Empty).TrimStart('\\');
    }

    public static string GetPathRelativeToTemporaryFolder(string path, Backup backup)
    {
        return path.Replace(ComposeTemporaryFolderPath(backup), string.Empty).TrimStart('\\');
    }

    public static string GetPathRelativeToFolder(string path, DirectoryInfo folder)
    {
        return path.Replace(folder.FullName, string.Empty).TrimStart('\\');
    }

    public static string GetFullPathRelativeToJobSource(string path, Job job)
    {
        return Path.Combine(job.Source, path);
    }

    public static string GetFullPathRelativeToJobDestination(string path, Job job)
    {
        return Path.Combine(job.Destination, path);
    }

    public static string GetFullPathRelativeToTemporaryFolder(string path, Backup backup)
    {
        return Path.Combine(ComposeTemporaryFolderPath(backup), path);
    }
}
