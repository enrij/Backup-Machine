using BackupMachine.Console;
using BackupMachine.Core;
using BackupMachine.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
               .ConfigureServices(services =>
                {
                    services.AddCore()
                            .AddInfrastructure()
                            .AddHostedService<BackupHostedService>();
                })
               .Build();

host.RunAsync();
