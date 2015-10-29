using MakerDen.Json;
using MakerDen.Sensor;
using System;

namespace MakerDen.Telemetry
{

    public sealed class SensorConnectTheDots : ISensorTelemetry
    {

        JSONWriter data = new JSONWriter();

        public ISensor Sensor { get; }
        IComponent component;
        public string Channel => string.Empty;


        public SensorConnectTheDots(ISensor sensor)
        {
            this.Sensor = sensor;
            this.component = sensor as IComponent;
        }


        /// <summary>
        /// ToJson function is used to convert sensor data into a JSON string to be sent to Azure Event Hub
        /// </summary>
        /// <returns>JSon String containing all info for sensor data</returns>
        public byte[] ToJson()
        {
            SerialiseData();
            return data.ToArray();
        }

        public override string ToString()
        {
            SerialiseData();
            return data.ToString();
        }

        private void SerialiseData()
        {

            data.Begin();

            data.AddProperty("value", Math.Round(Sensor.Value[0], 1).ToString());
            for (int i = 0; i < Sensor.Value.Length; i++)
            {
                data.AddProperty("value" + i.ToString(), Math.Round(Sensor.Value[i], 1).ToString());
            }
            data.AddProperty("measurename", component.Name);
            data.AddProperty("unitofmeasure", Sensor.UnitofMeasure);
            data.AddProperty("timecreated", Util.CorrectedUtcTime.ToString("o"));
            data.AddProperty("localtime", Util.CorrectedUtcTime.ToLocalTime().ToString());
            data.AddProperty("guid", Util.UniqueDeviceId);
            data.AddProperty("displayname", component.Name);
            data.AddProperty("organization", ConfigurationManager.Organisation);
            data.AddProperty("location", Sensor.GeoLocation);

            data.End();
        }
    }
}

