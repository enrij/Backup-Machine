using BackupMachine.Core.Entities;
using BackupMachine.Infrastructure.Persistence.Converters;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.Infrastructure.Persistence;

public class BackupMachineContext : DbContext
{
    public BackupMachineContext(DbContextOptions<BackupMachineContext> options)
        : base(options)
    {
    }

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Backup> Backups => Set<Backup>();
    public DbSet<BackupFolder> Folders => Set<BackupFolder>();
    public DbSet<BackupFile> Files => Set<BackupFile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BackupMachineContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DirectoryInfo>()
                            .HaveConversion<DirectoryInfoConverter>();
    }
}
