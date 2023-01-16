using LibreHardwareMonitor.Hardware;
namespace HardwareSensorsToMQTT;

public static class MeasurementUnit
{
    public static string SensorMeasurementUnit(ISensor sensor) => SensorMeasurementUnit(sensor.SensorType);

    public static string SensorMeasurementUnit(SensorType sensorType)
    {
        // for the most part this is from https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/ISensor.cs
        switch (sensorType)
        {
            case SensorType.Voltage:
                return "V";
            case SensorType.Current:
                return "A";
            case SensorType.Power:
                return "W";
            case SensorType.Clock:
                return "MHz";
            case SensorType.Temperature:
                return "°C";
            case SensorType.Load:
            case SensorType.Control:
            case SensorType.Level:
                return "%";
            case SensorType.Frequency:
                return "Hz";
            case SensorType.Fan:
                return "RPM";
            case SensorType.Flow:
                return "L/h";
            case SensorType.Factor:
                return "1"; // what?
            case SensorType.Data:
                return "GB"; // 2^30 Bytes
            case SensorType.SmallData:
                return "MB"; // 2^20 Bytes
            case SensorType.Throughput:
                return "B/s";
            case SensorType.TimeSpan:
                return "s"; // seconds
            case SensorType.Energy:
                return "mWh";
            default:
                return "";
        }
    }

    public static string HomeAssistantDeviceClass(SensorType sensorType)
    {
        // for the most part this is from https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/ISensor.cs
        switch (sensorType)
        {
            case SensorType.Voltage:
                return "voltage";
            case SensorType.Current:
                return "current";
            case SensorType.Power:
                return "power";
            case SensorType.Temperature:
                return "temperature";

            // return "power_factor";
            case SensorType.Frequency:
                return "frequency";
            case SensorType.Data:
            case SensorType.SmallData:
                return "data_size";
            case SensorType.Throughput:
                return "data_rate";
            case SensorType.TimeSpan:
                return "duration"; // seconds
            case SensorType.Energy:
                return "energy";
            case SensorType.Fan:  // rpm isn't supported in Home Assistant?
            case SensorType.Clock:
            case SensorType.Flow:
            case SensorType.Load:
            case SensorType.Control:
            case SensorType.Level:
            case SensorType.Factor: // still don't understand this one
            default:
                return "None";
        }
    }
}