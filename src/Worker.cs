using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Options;

namespace HardwareSensorsToMQTT;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //     await Task.Delay(1000, stoppingToken);
        // }
        var availableHardwareSensors = HardwareSensors.GetAllSensors();
        _logger.LogDebug("Found {0} hardware sensors", new[]
        {
            availableHardwareSensors.Count()
        });

        var sensorWrappers = new List<SensorWrapper>();
        var sensorsToCareAbout = ConfigToSensors(_configuration.GetSection("SensorsToPublish").GetChildren());
        _logger.LogDebug("Looking for '{0}' sensors", new object[] {
                String.Join(",", sensorsToCareAbout.Select(x => x.Id))
            });

        foreach (var item in sensorsToCareAbout)
        {
            var regex = new Regex(item.Id);
            var matchingSensors = availableHardwareSensors
                .Where(x => regex.IsMatch(x.Identifier.ToString()))
                .Select(x => new SensorWrapper(x, item.Interval));

            _logger.LogDebug("'{0}' matched sensor(s) '{1}' hardware sensors", item.Id, String.Join(", ", matchingSensors.Select(x => $"{x.FriendlyName} [{x.Id}]")));
    

            if(!matchingSensors.Any()) {
                _logger.LogWarning("Pattern '{0}' matched zero sensors. Could be a misconfiguration.", item.Id);
            }

            sensorWrappers.AddRange(matchingSensors);
        }

        var tasks = sensorWrappers.Select(x => new SensorTimer(x).StartTimerAsync(cancellationToken));
        await Task.WhenAll(tasks);
    }

    private static IEnumerable<SensorToPublishConfiguration> ConfigToSensors(IEnumerable<IConfigurationSection> input)
    {
        foreach (var item in input)
        {
            var id = item.GetValue<string>("id") ?? throw new ArgumentNullException("Sensor id is null");
            yield return new SensorToPublishConfiguration(id)
            {
                Interval = item.GetValue<TimeSpan>("interval")
            };
        }
    }
}
