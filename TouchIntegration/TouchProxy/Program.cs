using TouchProxy;

/// <summary>
/// Entry point to the Proxy Service application
/// </summary>

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<ProxyService>();
        services.AddHostedService<ProxyWorker>();
    })
    .Build();

await host.RunAsync();
