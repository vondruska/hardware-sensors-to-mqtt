using HardwareSensorsToMQTT;

var builder = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "HardwareSensorsToMQTT";
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.Configure<IEnumerable<SensorToPublishConfiguration>>(hostContext.Configuration.GetSection("SensorsToPublish"));
    });

builder.Build().Run();
