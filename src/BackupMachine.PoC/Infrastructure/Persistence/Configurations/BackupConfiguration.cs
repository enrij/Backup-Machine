using BackupMachine.PoC.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackupMachine.PoC.Infrastructure.Persistence.Configurations;

public class BackupConfiguration : IEntityTypeConfiguration<Backup>
{
    public void Configure(EntityTypeBuilder<Backup> builder)
    {
        builder.ToTable("Backups");

        builder.HasKey(backup => backup.Id);

        builder.HasOne(backup => backup.Job)
               .WithMany(job => job.Backups)
               .HasForeignKey(backup => backup.JobId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
