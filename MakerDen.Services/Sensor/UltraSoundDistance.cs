using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakerDen.Command;

namespace MakerDen.Sensor
{
    public class UltraSoundDistance : Glovebox.IoT.Devices.Sensors.Distance.HCSR04, ISensor, IComponent
    {
        public UltraSoundDistance(byte trig_Pin, byte echo_Pin) : base(trig_Pin, echo_Pin)
        {
            Value = new double[(int)ValuesPerSample];
        }

        #region Sensor

        public ValuesPerSample ValuesPerSample => ValuesPerSample.One;

        public string UnitofMeasure => "cm";

        public int SampleRateMilliseconds { get; set; } = 200;

        public string GeoLocation { get; set; } = ConfigurationManager.Location;


        public Trigger TriggerState { get; set; } = Trigger.Disabled;


        public int TriggerThreshold { get; set; } = 30;

        public double[] Value { get; }


        public void Measure()
        {
            double distance = 0;
            TriggerState = Trigger.None;

            if (this.GetDistanceToObstacle(ref distance))
            {
                Value[0] = distance;

                TriggerState = distance < TriggerThreshold ? Trigger.Triggered : Trigger.None;
            }
        }


        #endregion Sensor



        #region Component

        public Class Class => Class.Sensor;

        public string Name { get; set; } = "distance";


        public string Type { get; set; } = "hcsr04";


        public void Action(IotAction action)
        {
        }

        #endregion Component

    }
}
