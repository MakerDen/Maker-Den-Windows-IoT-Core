using MakerDen.Command;
using System;

namespace MakerDen.Sensor
{
    public class MemSensor : Glovebox.IoT.Devices.Sensors.AppMemoryUsage, ISensor, IComponent
    {

        public MemSensor()
        {
            Value = new double[(int)ValuesPerSample];
        }


        #region Sensor

        public ValuesPerSample ValuesPerSample => ValuesPerSample.One;

        public string UnitofMeasure => "KiB";

        public int SampleRateMilliseconds { get; set; } = 5000;

        public string GeoLocation { get; set; } = ConfigurationManager.Location;


        public Trigger TriggerState { get; set; } = Trigger.Disabled;


        public int TriggerThreshold { get; set; } = 0;

        public double[] Value { get; }


        public void Measure()
        {
            if (Value == null)
            {
                throw new NullReferenceException("Sensor Value field not Instantiated ");
            }
            Value[0] = this.Current;
        }


        #endregion Sensor



        #region Component

        public Class Class => Class.Sensor;

        public string Name { get; set; } = "memory";


        public string Type { get; set; } = "mem";


        public void Action(IotAction action)
        {
        }

        #endregion Component


        public void Dispose()
        {
        }
    }
}
