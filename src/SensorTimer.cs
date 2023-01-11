namespace HardwareSensorsToMQTT
{
    public class SensorTimer
    {
        private readonly SensorWrapper _sensor;
        public SensorTimer(SensorWrapper sensorWrapper)
        {
            _sensor = sensorWrapper;
        }

        public async Task StartTimerAsync(CancellationToken cancellationToken)
        {
            Update();

            var timer = new PeriodicTimer(_sensor.PublishInterval);

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                Update();
            }
        }

        private void Update()
        {
            _sensor.Update();
            
            Console.WriteLine($"[{DateTime.Now.ToString()}] - {_sensor.Value}");
        }
    }
}