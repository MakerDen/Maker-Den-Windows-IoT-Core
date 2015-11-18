using Glovebox.IoT.Devices.Sensors;
using MakerDen.Command;
using System;
using Windows.Devices.Adc;

namespace MakerDen.Sensor
{
    public class TempSensor : MCP9700A, ISensor, IComponent
    {

        public TempSensor(AdcChannel channel, int referenceMillivolts=3300) : base(channel, referenceMillivolts)
        {
            Value = new double[(int)ValuesPerSample];
        }

        #region Sensor

        public ValuesPerSample ValuesPerSample => ValuesPerSample.One;

        public string UnitofMeasure => "c";

        public int SampleRateMilliseconds { get; set; } = 4000;

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
            Value[0] = this.Temperature.DegreesCelsius;
        }


        #endregion Sensor



        #region Component

        public Class Class => Class.Sensor;
        public string Name { get; set; } = "temperature";


        public string Type { get; set; } = "temp";


        public void Action(IotAction action)
        {
        }

        #endregion Component


    }
}
