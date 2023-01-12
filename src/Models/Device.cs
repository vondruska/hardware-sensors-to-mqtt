using System.Text.Json.Serialization;

namespace HardwareSensorsToMQTT.Models.HomeAssistant;

public class Device
{
    [JsonPropertyName("configuration_url")]
    public Uri? ConfigurationUrl { get; set; }

    [JsonPropertyName("connections")]
    public IEnumerable<string>? Connections { get; set; }

    [JsonPropertyName("hw_version")]
    public string? HardwareVersion { get; set; }

    [JsonPropertyName("identifiers")]
    public IEnumerable<string?>? Identifiers { get; set; }

    [JsonPropertyName("manufacturer")]
    public string? Manufacturer { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("suggested_area")]
    public string? SuggestedArea { get; set; }

    [JsonPropertyName("sw_version")]
    public string? SoftwareVersion { get; set; }

    [JsonPropertyName("via_device")]
    public string? ViaDevice { get; set; }

}