using BackupMachine.Console;
using BackupMachine.Core;
using BackupMachine.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
               .UseEnvironment("Development") // Well... Maybe there is a better way but this is only a PoC sooo... Fuck it :)
               .ConfigureServices(services =>
                {
                    services.AddCore()
                            .AddInfrastructure()
                            .AddHostedService<BackupHostedService>();
                })
               .Build();

host.RunAsync();
