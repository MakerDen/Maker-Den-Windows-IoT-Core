using MakerDen.Command;
using MakerDen.Sensor;
using MakerDen.Telemetry;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MakerDen
{
    public class SensorMgr : IDisposable
    {

        public enum Actions { Start, Stop, Measure };
        ISensorTelemetry SensorData;
        public ISensor sensor { get; private set; }
        public IComponent component { get; }

        private int sensorId;

        private static int SensorCount = -1; // -1 so after first increment the result is 0
        public static string DeviceName { get; set; } = "RPi";

        public enum Sampling
        {
            Auto,
            Manual
        }

        Sampling sampling = Sampling.Auto;


        public delegate uint SensorEventHandler(object sender, EventArgs e);
        public event SensorEventHandler OnAfterMeasurement;
        public event SensorEventHandler OnBeforeMeasurement;


        public class SensorIdEventArgs : EventArgs
        {
            public readonly int id;
            public SensorIdEventArgs(int id)
            {
                this.id = id;
            }
        }

        public class SensorItemEventArgs : EventArgs
        {
            public ISensorTelemetry SensorTelemetry { get; private set; }

            public SensorItemEventArgs(ISensorTelemetry sensorTelemetry)
            {
                this.SensorTelemetry = sensorTelemetry;
            }
        }


        public SensorMgr(ISensor sensor, Sampling sampling = Sampling.Auto)
        {

            this.sensor = sensor;
            this.component = sensor as IComponent;
            this.sampling = sampling;

            IotActionManager.AddComponent(component);

            sensorId = Interlocked.Increment(ref SensorCount);

            switch (ConfigurationManager.CloudConnection)
            {
                case ConfigurationManager.CloudMode.MQTT:
                    SensorData = new SensorMqtt(sensor);
                    break;
                case ConfigurationManager.CloudMode.EventHub:
                    SensorData = new SensorConnectTheDots(sensor);
                    break;
                case ConfigurationManager.CloudMode.IoT_Hub:
                    SensorData = new SensorIoTHub(sensor, DeviceName);
                    break;
                default:
                    SensorData = new SensorMqtt(sensor);
                    break;
            }

            if (sampling == Sampling.Auto) { StartMeasuring(); }
        }

        public double[] Measure()
        {
            sensor.Measure();
            return sensor.Value;
        }


        public void StartMeasuring()
        {
            if (sensor.SampleRateMilliseconds > 0)
            {
                Task.Run(() => MeasureThread());
            }
        }


        private void MeasureThread()
        {
            while (true)
            {
                OnBeforeMeasurement?.Invoke(this, new SensorIdEventArgs(sensorId));

                sensor.Measure();

                if (sensor.TriggerState == Trigger.Disabled || sensor.TriggerState == Trigger.Triggered)
                {
                    OnAfterMeasurement?.Invoke(this, new SensorItemEventArgs(SensorData));
                }

                Util.Delay(sensor.SampleRateMilliseconds);
            }
        }


        public override string ToString()
        {
            return SensorData.ToString();
        }


        public void Dispose()
        {
            IotActionManager.RemoveComponent(sensor as IComponent);
            sensor = null;
        }
    }
}
