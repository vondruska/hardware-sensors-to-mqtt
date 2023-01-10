namespace HardwareSensorsToMQTT
{
    public class SensorTimer
    {
        private readonly Monitor _monitor;
        private readonly string _id;
        private readonly TimeSpan _interval;
        public SensorTimer(Monitor monitor, string id, TimeSpan interval)
        {
            _monitor = monitor;
            _id = id;
            _interval = interval;
        }

        public async Task StartTimerAsync(CancellationToken cancellationToken)
        {
            Update();

            var timer = new PeriodicTimer(_interval);

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                Update();
            }
        }

        private void Update()
        {
            var sensorResult = _monitor.GetSensorValue(_id);
            Console.WriteLine($"[{DateTime.Now.ToString()}] - {sensorResult}");
        }
    }
}