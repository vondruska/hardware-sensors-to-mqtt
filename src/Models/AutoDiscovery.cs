using System.Text.Json.Serialization;

namespace HardwareSensorsToMQTT.Models.HomeAssistant;

public class AutoDiscovery
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("device_class")]
    public string? DeviceClass { get; set; }
    [JsonPropertyName("unit_of_measurement")]
    public string? UnitOfMeasurement { get; set; }
    [JsonPropertyName("state_topic")]
    public string? StateTopic { get; set; }

    [JsonPropertyName("device")]
    public Device? Device { get; set; }
    [JsonPropertyName("force_update")]
    public bool ForceUpdate { get; set; }

    [JsonPropertyName("payload_available")]
    public string? PayloadAvailable { get; set; }

    [JsonPropertyName("payload_not_available")]
    public string? PayloadNotAvailable { get; set; }

    [JsonPropertyName("availability_topic")]
    public string? AvailabilityTopic { get; set; }
}