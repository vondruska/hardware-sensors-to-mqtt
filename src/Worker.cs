using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;

namespace HardwareSensorsToMQTT;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IOptions<HardwareSensorsToMQTTConfiguration> _configuration;
    private readonly IManagedMqttClient _mqttClient;

    public Worker(ILogger<Worker> logger, ILoggerFactory loggerFactory, IOptions<HardwareSensorsToMQTTConfiguration> configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _mqttClient = new MqttFactory(new MqttLogger(loggerFactory)).CreateManagedMqttClient();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await StartMqttClientAsync();
        var mqttPublisher = new MqttPublisher(_configuration.Value, _mqttClient);

        var availableHardwareSensors = HardwareSensors.GetAllSensors();
        _logger.LogDebug("Found {0} hardware sensors", new[]
        {
            availableHardwareSensors.Count()
        });

        var sensorWrappers = new List<SensorWrapper>();
        var sensorsToCareAbout = _configuration.Value.SensorsToPublish;
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


            if (!matchingSensors.Any())
            {
                _logger.LogWarning("Pattern '{0}' matched zero sensors. Could be a misconfiguration.", item.Id);
            }

            sensorWrappers.AddRange(matchingSensors);
        }

        if (_configuration.Value.Mqtt.EnableHomeAssistantAutoDiscovery)
        {
            await mqttPublisher.PublishHomeAssistantAutoDiscover(sensorWrappers);
        }

        var tasks = sensorWrappers.Select(x => new SensorTimer(x, mqttPublisher).StartTimerAsync(cancellationToken));
        await Task.WhenAll(tasks);
    }

    public async override Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);

        await _mqttClient.StopAsync();
    }

    private async Task StartMqttClientAsync()
    {
        var options = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            .WithClientOptions(new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(_configuration.Value.Mqtt.Host, _configuration.Value.Mqtt.Port)
                .WithCredentials(_configuration.Value.Mqtt.Username, _configuration.Value.Mqtt.Password)
                .Build())
            .Build();

        await _mqttClient.StartAsync(options);
    }

    private async Task<IManagedMqttClient> MqttClient()
    {
        // Setup and start a managed MQTT client.
        var options = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            .WithClientOptions(new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(_configuration.Value.Mqtt.Host, _configuration.Value.Mqtt.Port)
                .WithCredentials(_configuration.Value.Mqtt.Username, _configuration.Value.Mqtt.Password)
                .Build())
            .Build();

        var mqttClient = new MqttFactory().CreateManagedMqttClient();
        await mqttClient.StartAsync(options);
        return mqttClient;
    }
}

public class MqttLogger : IMqttNetLogger
{
    private readonly ILogger _logger;
    public MqttLogger(ILoggerFactory factory)
    {
        _logger = factory.CreateLogger("HardwareSensorsToMQTT.MQTT");
    }
    public bool IsEnabled => true;

    public void Publish(MqttNetLogLevel logLevel, string source, string message, object[] parameters, Exception exception)
    {
        switch (logLevel)
        {
            case MqttNetLogLevel.Error:
                _logger.LogError(exception, message, source, parameters);
                break;
            case MqttNetLogLevel.Info:
                _logger.LogInformation(exception, message, source, parameters);
                break;
            case MqttNetLogLevel.Warning:
                _logger.LogWarning(exception, message, source, parameters);
                break;
            case MqttNetLogLevel.Verbose:
                _logger.LogDebug(exception, message, source, parameters);
                break;
        }
    }
}