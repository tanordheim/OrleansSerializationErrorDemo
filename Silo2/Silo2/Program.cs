using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Silo2.Abstractions;

using var host = new HostBuilder()
    .UseOrleans(builder =>
    {
        builder
            .UseLocalhostClustering(siloPort: 11111, gatewayPort: 30000, serviceId: "silo2")
            .AddMemoryGrainStorageAsDefault();
    })
    .ConfigureLogging((_, loggingBuilder) =>
    {
        loggingBuilder.AddConsole();
    })
    .Build();
await host.StartAsync();

var grainFactory = host.Services.GetRequiredService<IGrainFactory>();

Console.WriteLine("press enter to try to fetch value from grain");
Console.ReadLine();

var grain = grainFactory.GetGrain<IStateGrain>("test");
var value = await grain.GetValue();
Console.WriteLine($"Got value: {value}");

Console.WriteLine("Press a key to terminate");
Console.ReadLine();
await host.StopAsync();