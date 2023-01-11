namespace HardwareSensorsToMQTT;

public class SensorToPublishConfiguration
{
    public SensorToPublishConfiguration(string id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }

    public string Id { get; set; }
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(3);
}