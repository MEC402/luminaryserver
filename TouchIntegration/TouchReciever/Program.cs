using TouchReceiver;

/// <summary>
/// Entry point to the Receiver Service application
/// </summary>
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<ReceiverService>();
        services.AddHostedService<ReceiverWorker>();
    })
    .Build();

await host.RunAsync();
