namespace HardwareSensorsToMQTT;

public interface IMqttPublisher
{
    Task PublishHomeAssistantAutoDiscover(IEnumerable<SensorWrapper> sensors);
    Task PublishSensorValue(string sensorId, float? value);
}