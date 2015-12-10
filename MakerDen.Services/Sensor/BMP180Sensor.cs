using MakerDen.Command;
using System;

namespace MakerDen.Sensor
{
    public class BMP180Sensor : Glovebox.IoT.Devices.Sensors.BMP180, ISensor, IComponent
    {

        public BMP180Sensor()
        {
            Value = new double[(int)ValuesPerSample];
        }


        #region Sensor

        public ValuesPerSample ValuesPerSample => ValuesPerSample.Two;

        public string[] UnitofMeasure => new string[]{"C, hPa"};


        public int SampleRateMilliseconds { get; set; } = 2000;

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

            Value[0] = Temperature.DegreesCelsius;
            Value[1] = Pressure.Hectopascals;
        }


        #endregion Sensor


        #region Component

        public Class Class => Class.Sensor;
        public string Name { get; set; } = "Temperature and Pressure";


        public string Type { get; set; } = "bmp180";


        public void Action(IotAction action)
        {
        }

        #endregion Component

    }
}
