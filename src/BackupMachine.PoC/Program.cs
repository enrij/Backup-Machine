using BackupMachine.PoC;
using BackupMachine.PoC.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                             })
                            .AddHostedService<BackupHostedService>()
                            .AddMediatR(typeof(BackupHostedService));
                })
               .Build();

host.RunAsync();
