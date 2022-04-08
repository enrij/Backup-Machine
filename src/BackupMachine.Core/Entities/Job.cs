namespace BackupMachine.Core.Entities;

public class Job
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;

    public virtual List<Backup> Backups { get; set; } = new();
}
