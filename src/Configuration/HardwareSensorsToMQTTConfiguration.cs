namespace HardwareSensorsToMQTT;

public class HardwareSensorsToMQTTConfiguration
{
    public IEnumerable<SensorToPublishConfiguration> SensorsToPublish { get; set; } = Enumerable.Empty<SensorToPublishConfiguration>();
    public string? Name { get; set; }
    public MqttConfiguration Mqtt { get; set; } = new MqttConfiguration();
}