using MakerDen.Json;
using MakerDen.Sensor;

namespace MakerDen.Telemetry
{
    public class SensorMqtt : ISensorTelemetry
    {

        JSONWriter jw = new JSONWriter();

        public ISensor Sensor { get; }
        IComponent component;
        public string Channel { get; private set; }

        static int MsgId { get; set; }

        string DeviceName { get; set; }


        public SensorMqtt(ISensor sensor, string deviceName)
        {
            this.Sensor = sensor;
            this.component = sensor as IComponent;
            this.DeviceName = deviceName;

            Channel = ConfigurationManager.MqttNameSpace + deviceName + "/" + component.Type;
        }


        public byte[] ToJson()
        {
            SerialiseData();
            return jw.ToArray();
        }

        public override string ToString()
        {
            SerialiseData();
            return jw.ToString();
        }

        private void SerialiseData()
        {
            jw.Begin();
            jw.AddProperty("Dev", DeviceName);
            jw.AddProperty("Type", component.Type);
            jw.AddProperty("Val", Sensor.Value, 2);
            jw.AddProperty("Unit", Sensor.UnitofMeasure);
            jw.AddProperty("Utc", Util.CorrectedUtcTime);
            if (Sensor.GeoLocation != string.Empty) { jw.AddProperty("Geo", Sensor.GeoLocation); }
            jw.AddProperty("Id", MsgId++);
            jw.End();
        }
    }
}
