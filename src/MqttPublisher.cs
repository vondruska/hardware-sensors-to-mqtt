using HardwareSensorsToMQTT.Models.HomeAssistant;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace HardwareSensorsToMQTT;

public class MqttPublisher : IMqttPublisher
{
    private const string APPLICATION_MQTT_PREFIX = "hardwaresensors";
    private const string STATE_SUFFIX = "state";
    private readonly HardwareSensorsToMQTTConfiguration _configuration;
    private readonly IManagedMqttClient _mqttClient;



    public MqttPublisher(HardwareSensorsToMQTTConfiguration configuration, IManagedMqttClient managedMqttClient)
    {
        _configuration = configuration;
        _mqttClient = managedMqttClient;
    }

    public async Task PublishHomeAssistantAutoDiscover(IEnumerable<SensorWrapper> sensors)
    {
        foreach (var sensor in sensors)
        {
            var model = new AutoDiscovery()
            {
                DeviceClass = sensor.HomeAssistantDeviceClass,
                Name = sensor.FriendlyName,
                UnitOfMeasurement = sensor.UnitOfMeasure,
                StateTopic = StateTopic(sensor.Id),
                Device = Device
            };

            // this is a ManagedMqttClient that enqueues messages internally to deal with connection failures
            // with an internal queue, just dump all the messages in there instead of awaiting them all
            await _mqttClient.EnqueueAsync(AutoDiscoverTopic(sensor.Id), JsonSerializer.Serialize(model));
        }
    }

    public Task PublishSensorValue(string sensorId, float? value)
        => _mqttClient.EnqueueAsync(StateTopic(sensorId), value.ToString());

    private string DeviceName => _configuration.Name ?? Environment.MachineName;

    private string StateTopic(string id) => $"{APPLICATION_MQTT_PREFIX}/{DeviceName}/{id.NormalizeIdForMqtt()}/state";
    private string AutoDiscoverTopic(string id) => $"{_configuration.Mqtt.HomeAssistantAutoDiscoverTopic}/sensor/{id.NormalizeIdForMqtt()}/config";

    private Device Device
    {
        get
        {
            return new Device
            {
                Identifiers = new string?[] {
                    DeviceName
                },
                Name = DeviceName
            };
        }
    }
}