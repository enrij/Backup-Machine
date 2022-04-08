using BackupMachine.Core.Interfaces;
using BackupMachine.Infrastructure.FileSystem;
using BackupMachine.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BackupMachine.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services.AddPooledDbContextFactory<BackupMachineContext>(options =>
                        {
                            options.ConfigureWarnings(builder =>
                            {
                                builder.Log(
                                    (RelationalEventId.CommandExecuted, LogLevel.Debug),
                                    (RelationalEventId.ConnectionOpened, LogLevel.Debug),
                                    (CoreEventId.ContextInitialized, LogLevel.Debug),
                                    (RelationalEventId.MigrationsNotFound, LogLevel.Debug)
                                );
                            });
                            options.UseSqlite(
                                "Data Source=C:\\Users\\EnricoBarbieri\\source\\repos\\Backup-Machine\\src\\BackupMachine.PoC\\BackupMachine.db",
                                optionsBuilder => { optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); }
                            );
                        })
                       .AddSingleton<IPersistenceService, PersistenceService>()
                       .AddSingleton<IFileSystemService, FileSystemService>();
    }
}
