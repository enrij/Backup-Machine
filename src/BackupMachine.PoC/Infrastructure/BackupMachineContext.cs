using BackupMachine.PoC.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace BackupMachine.PoC.Infrastructure;

public class BackupMachineContext : DbContext
{
    public BackupMachineContext(DbContextOptions<BackupMachineContext> options)
        : base(options)
    {
    }

    public DbSet<Job> Jobs => Set<Job>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BackupMachineContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
    }
}
