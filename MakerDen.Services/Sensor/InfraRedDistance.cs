using MakerDen.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakerDen.Sensor
{
    public class InfraRedDistance : Glovebox.IoT.Devices.Sensors.Distance.InfraRed, ISensor, IComponent
    {
        public InfraRedDistance(byte IR_Pin) : base(IR_Pin)
        {
            Value = new double[(int)ValuesPerSample];
        }

        #region Sensor

        public ValuesPerSample ValuesPerSample => ValuesPerSample.One;

        public string UnitofMeasure => "cm";

        public int SampleRateMilliseconds { get; set; } = 50;

        public string GeoLocation { get; set; } = ConfigurationManager.Location;


        public Trigger TriggerState { get; set; } = Trigger.Disabled;


        public int TriggerThreshold { get; set; } = 10;

        public double[] Value { get; }


        public void Measure()
        {
            double distance = 0;
            TriggerState = Trigger.None;

            if (this.GetDistanceToObstacle(ref distance))
            {
                Value[0] = TriggerThreshold;

                TriggerState = Trigger.Triggered;
            }
        }


        #endregion Sensor



        #region Component

        public Class Class => Class.Sensor;

        public string Name { get; set; } = "distance";


        public string Type { get; set; } = "infrared";


        public void Action(IotAction action)
        {
        }

        #endregion Component

    }
}
