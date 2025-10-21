using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PlayerService.Models;
using PlayerService.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
        {
            services.AddSingleton<PlayerState>();
            services.AddHostedService<ConsoleInterfaceService>();
        }).Build();

await host.RunAsync();
