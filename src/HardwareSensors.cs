using LibreHardwareMonitor.Hardware;

namespace HardwareSensorsToMQTT
{
    public class HardwareSensors : IDisposable
    {
        private readonly Computer _computer;

        public HardwareSensors()
        {
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true
            };

            _computer.Open();
            _computer.Accept(new UpdateVisitor());
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

        public IEnumerable<ISensor> GetAllSensors()
        {
            var returnList = new List<ISensor>();

            foreach (IHardware hardware in _computer.Hardware)
            {
                // Console.WriteLine("Hardware: {0}", hardware.Name);

                foreach (IHardware subhardware in hardware.SubHardware)
                {
                    // Console.WriteLine("\tSubhardware: {0}", subhardware.Name);

                    foreach (ISensor sensor in subhardware.Sensors)
                    {
                        returnList.Add(sensor);
                        //Console.WriteLine("\t\tSensor: '{0}', value: '{1}', id '{2}'", sensor.Name, sensor.Value, sensor.Identifier);
                    }
                }

                foreach (ISensor sensor in hardware.Sensors)
                {
                    returnList.Add(sensor);
                    //Console.WriteLine("\tSensor: '{0}', value: '{1}', id '{2}'", sensor.Name, sensor.Value, sensor.Identifier);
                }
            }

            return returnList;
        }

        public void Dispose()
        {
            _computer.Close();
        }
    }
}