using LibreHardwareMonitor.Hardware;

namespace HardwareSensorsToMQTT;
public class SensorWrapper : ISensorWrapper, IComparable<SensorWrapper>
{
    private readonly ISensor _sensor;
    public SensorWrapper(ISensor sensor, TimeSpan publishInterval)
    {
        _sensor = sensor;
        PublishInterval = publishInterval;
    }

    public string Id => _sensor.Identifier.ToString();
    public string FriendlyName => $"{_sensor.Hardware.Name} {_sensor.Name}";
    public float? Value => _sensor.Value;
    public void Update() => _sensor.Hardware.Update();


    public string UnitOfMeasure => MeasurementUnit.SensorMeasurementUnit(_sensor.SensorType);
    public string HomeAssistantDeviceClass => MeasurementUnit.HomeAssistantDeviceClass(_sensor.SensorType);
    public TimeSpan PublishInterval { get; private set; }

    public int CompareTo(SensorWrapper? other)
    {
        if (other == null)
            return 1;

        return string.Compare(Id,
                              other.Id,
                              StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        SensorWrapper? otherWrapper = obj as SensorWrapper;
        if (otherWrapper == null)
            return false;

        return Id == otherWrapper.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(SensorWrapper? wrapper1, SensorWrapper? wrapper2)
    {
        if (wrapper1 is null && wrapper2 is null)
            return true;

        return wrapper1 is not null && wrapper1.Equals(wrapper2);
    }

    public static bool operator !=(SensorWrapper? wrapper1, SensorWrapper? wrapper2)
    {
        return !(wrapper1 == wrapper2);
    }

}

public interface ISensorWrapper
{

}