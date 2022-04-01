using BackupMachine.PoC;
using BackupMachine.PoC.Domain.Services;
using BackupMachine.PoC.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
               .ConfigureServices(services =>
               {
                   services.AddPooledDbContextFactory<BackupMachineContext>(options =>
                   {
                       options.UseSqlite(
                           "Data Source=C:\\Users\\EnricoBarbieri\\source\\repos\\Backup-Machine\\src\\BackupMachine.PoC\\BackupMachine.db",
                           optionsBuilder => { optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); }
                       );
                   });
                   services.AddHostedService<BackupHostedService>();

                   services.AddSingleton<BackupService>();
               })
               .Build();

host.RunAsync();
