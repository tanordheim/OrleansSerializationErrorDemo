using System.Net;
using Microsoft.Extensions.Hosting;

using var host = new HostBuilder()
    .UseOrleans(builder =>
    {
        builder
            .UseLocalhostClustering(siloPort: 11112, gatewayPort: 30001, primarySiloEndpoint: IPEndPoint.Parse("127.0.0.1:11111"), serviceId: "silo1")
            .AddMemoryGrainStorageAsDefault();
    })
    .Build();
await host.StartAsync();
Console.WriteLine("Press a key to terminate");
Console.ReadLine();
await host.StopAsync();