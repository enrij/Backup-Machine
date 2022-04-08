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

        builder.HasOne(backup => backup.PreviousBackup)
               .WithMany() // TODO: This is a lie but I don't want to create a useless property (and a circular reference)
               .HasForeignKey(backup => backup.PreviousBackupId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
