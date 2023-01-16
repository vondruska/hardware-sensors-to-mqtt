using HardwareSensorsToMQTT;


if (args.Length == 1)
{

    if (args[0] == "list-sensors")
    {
        Console.WriteLine("Dumping all hardware sensors available");
        var hardwareSensors = new HardwareSensors();
        var sensors = hardwareSensors.GetAllSensors();

        foreach (var item in sensors.OrderBy(x => x.Hardware.Name).ThenBy(x => x.SensorType).ThenBy(x => x.Name))
        {
            Console.WriteLine($"Name = {item.Name} {item.SensorType} || Hardware = {item.Hardware.Name} || Id = {item.Identifier} || Value = {item.Value}");
        }

        Environment.Exit(0);
    }
    if (args[0] == "configured-sensors")
    {
        Console.WriteLine("Dumping all publishable sensors");
        Console.WriteLine("Not implemented... yet");
        Environment.Exit(0);
    }
}


var builder = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "HardwareSensorsToMQTT";
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.Configure<HardwareSensorsToMQTTConfiguration>(hostContext.Configuration.GetSection("HardwareSensorsToMQTT"));
    });

builder.Build().Run();
