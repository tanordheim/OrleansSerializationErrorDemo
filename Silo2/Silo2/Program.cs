using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Silo2.Grains;

using var host = new HostBuilder()
    .UseOrleans(builder =>
    {
        builder
            .UseLocalhostClustering(siloPort: 11111, gatewayPort: 30000, serviceId: "silo2")
            .AddMemoryGrainStorageAsDefault();
    })
    .Build();
await host.StartAsync();

var grainFactory = host.Services.GetRequiredService<IGrainFactory>();

Console.WriteLine("press enter to try to fetch value from grain");
Console.ReadLine();

var grain = grainFactory.GetGrain<IStateGrain>("test");
var value = await grain.GetValue();
Console.WriteLine($"Got value: {value}");

await host.StopAsync();