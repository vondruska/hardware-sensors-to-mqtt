namespace HardwareSensorsToMQTT
{
    public class SensorTimer
    {
        private readonly SensorWrapper _sensor;
        private readonly IMqttPublisher _mqtt;
        public SensorTimer(SensorWrapper sensorWrapper, IMqttPublisher mqtt)
        {
            _sensor = sensorWrapper;
            _mqtt = mqtt;
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

        private async void Update()
        {
            _sensor.Update();
            await _mqtt.PublishSensorValue(_sensor.Id, _sensor.Value);

            Console.WriteLine($"[{DateTime.Now.ToString()}] - {_sensor.Value}");
        }
    }
}