namespace HardwareSensorsToMQTT
{
    public class SensorToMonitor
    {
        public SensorToMonitor(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public string Id { get; set; }
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(3);
    }
}