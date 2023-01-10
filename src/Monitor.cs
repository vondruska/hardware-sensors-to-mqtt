using LibreHardwareMonitor.Hardware;

namespace HardwareSensorsToMQTT
{
    public class Monitor
    {
        Dictionary<string, ISensor> _sensorsToMonitor = new Dictionary<string, ISensor>();
        public Monitor(ICollection<SensorToMonitor> sensors)
        {
            var sensorIdsToCollect = sensors.Select(x => x.Id).ToList();
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true
            };

            computer.Open();
            computer.Accept(new UpdateVisitor());

            foreach (IHardware hardware in computer.Hardware)
            {
                foreach (IHardware subhardware in hardware.SubHardware)
                {
                    foreach (ISensor sensor in subhardware.Sensors)
                    {
                        if (sensorIdsToCollect.Contains(sensor.Identifier.ToString()))
                        {
                            _sensorsToMonitor.TryAdd(sensor.Identifier.ToString(), sensor);
                        }
                    }
                }

                foreach (ISensor sensor in hardware.Sensors)
                {
                    if (sensorIdsToCollect.Contains(sensor.Identifier.ToString()))
                    {
                        _sensorsToMonitor.TryAdd(sensor.Identifier.ToString(), sensor);
                    }
                }
            }

            computer.Close();
        }

        public float? GetSensorValue(string id)
        {
            if (!_sensorsToMonitor.TryGetValue(id, out ISensor? sensor))
            {
                throw new KeyNotFoundException($"Sensor {id} not found");
            }

            sensor.Hardware.Update();
            return sensor.Value;
        }



        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }

        public static void DumpAll()
        {
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true
            };

            computer.Open();
            computer.Accept(new UpdateVisitor());

            foreach (IHardware hardware in computer.Hardware)
            {
                Console.WriteLine("Hardware: {0}", hardware.Name);

                foreach (IHardware subhardware in hardware.SubHardware)
                {
                    Console.WriteLine("\tSubhardware: {0}", subhardware.Name);

                    foreach (ISensor sensor in subhardware.Sensors)
                    {
                        Console.WriteLine("\t\tSensor: '{0}', value: '{1}', id '{2}'", sensor.Name, sensor.Value, sensor.Identifier);
                    }
                }

                foreach (ISensor sensor in hardware.Sensors)
                {
                    Console.WriteLine("\tSensor: '{0}', value: '{1}', id '{2}'", sensor.Name, sensor.Value, sensor.Identifier);
                }
            }

            computer.Close();
        }
    }
}