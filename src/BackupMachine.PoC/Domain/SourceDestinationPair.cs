namespace BackupMachine.PoC.Domain;

/// <summary>
/// Holds a source root path and destination root path for a backup operation
/// </summary>
public record SourceDestinationPair(
    string Source,
    string Destination);
