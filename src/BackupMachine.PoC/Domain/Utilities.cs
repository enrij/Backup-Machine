using BackupMachine.PoC.Domain.Entities;

namespace BackupMachine.PoC.Domain;

public static class Utilities
{
    public static string ComposeBackupArchiveName(Backup backup)
    {
        return $"BackupMachine-{backup.Timestamp:yyyy MM dd HH mm ss}.zip";
    }

    public static string GetBackupDestinationRootFolderPath(Backup backup)
    {
        //return Path.Combine(backup.Job.Destination, $"{backup.Timestamp:yyyy MM dd HH mm ss}");
        return backup.Job.Destination;
    }

    public static string GetPathRelativeToJobSource(DirectoryInfo folder, Job job)
    {
        return folder.FullName.Replace(job.Source, string.Empty).TrimStart('\\');
    }

    public static string GetBackupFolderDestinationPath(BackupFolder folder)
    {
        return Path.Combine(GetBackupDestinationRootFolderPath(folder.Backup), folder.RelativePath);
    }

    public static string GetBackupFileDestinationPath(BackupFile file)
    {
        return Path.Combine(GetBackupDestinationRootFolderPath(file.Backup), file.BackupFolder.RelativePath, file.Name);
    }
}
