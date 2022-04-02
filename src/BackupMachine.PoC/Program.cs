using BackupMachine.PoC;
using BackupMachine.PoC.Domain.Services;
using BackupMachine.PoC.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
               .UseEnvironment("Development") // Well... Maybe there is a better way but this is only a PoC sooo... Fuck it :)
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
