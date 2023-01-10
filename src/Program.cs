using Microsoft.Extensions.Configuration;
using HardwareSensorsToMQTT;
if (args.Length > 0 && args[0] == "dump")
{
    HardwareSensorsToMQTT.Monitor.DumpAll();
    Environment.Exit(0);
}




var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false);

var configuration = builder.Build();
var sensors = configuration.GetSection("sensors").GetChildren();

var sensorsToMonitor = ConfigToSensors(sensors);

var monitor = new HardwareSensorsToMQTT.Monitor(sensorsToMonitor.ToList());
// TODO: read sensors, spin up timers

var timers = new List<Task>();
foreach (var item in sensorsToMonitor)
{
    timers.Add(new SensorTimer(monitor, item.Id, item.Interval).StartTimerAsync(CancellationToken.None));
}

await Task.WhenAll(timers);

public static partial class Program
{
    private static IEnumerable<SensorToMonitor> ConfigToSensors(IEnumerable<IConfigurationSection> input)
    {
        foreach (var item in input)
        {
            var id = item.GetValue<string>("id") ?? throw new ArgumentNullException("Sensor id is null");
            yield return new SensorToMonitor(id)
            {
                Interval = item.GetValue<TimeSpan>("interval")
            };
        }
    }
}
