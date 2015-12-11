using MakerDen.Command;
using System;
using Windows.Devices.Adc;

namespace MakerDen.Sensor
{
    public sealed class LightSensor : Glovebox.IoT.Devices.Sensors.Ldr, ISensor, IComponent
    {

        public LightSensor(AdcChannel channel) : base(channel)
        {
            Value = new double[(int)ValuesPerSample];
        }


        #region Sensor

        public ValuesPerSample ValuesPerSample => ValuesPerSample.One;

        public string[] UnitofMeasure => new string[] { "light" };

        public int SampleRateMilliseconds { get; set; } = 1000;

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

            Value[0] = (1 - this.ReadRatio) * 100;
        }


        #endregion Sensor




        #region Component
        public Class Class => Class.Sensor;

        public string Type => "light";

        public string Name => "light";



        public void Action(IotAction action)
        {

        }

        #endregion Component

    }
}
