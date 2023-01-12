namespace HardwareSensorsToMQTT;

public class MqttConfiguration
{
    public bool EnableHomeAssistantAutoDiscovery { get; set; } = true;
    public string HomeAssistantAutoDiscoverTopic { get; set; } = "homeassistant";
    public string? Host { get; set; }
    public int Port { get; set; } = 1883;
    public string? Username { get; set; }
    public string? Password { get; set; }
}