using HardwareSensorsToMQTT;


if (args.Length == 1)
{

    if (args[0] == "list-sensors")
    {
        Console.WriteLine("Dumping all hardware sensors available");
        var sensors = HardwareSensors.GetAllSensors();

        foreach (var item in sensors)
        {
            Console.WriteLine($"Name = {item.Name} || Hardware = {item.Hardware.Name} || Id = {item.Identifier} || Value = {item.Value}");
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
