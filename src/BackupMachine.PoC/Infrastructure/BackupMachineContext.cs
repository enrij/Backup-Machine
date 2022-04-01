using BackupMachine.PoC.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Infrastructure;

public class BackupMachineContext : DbContext
{
    public BackupMachineContext(DbContextOptions<BackupMachineContext> options)
        : base(options)
    {
    }

    public DbSet<FolderCopy> Folders
    {
        get { return Set<FolderCopy>(); }
    }

    public DbSet<Snapshot> Snapshots
    {
        get { return Set<Snapshot>(); }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BackupMachineContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
    }
}
