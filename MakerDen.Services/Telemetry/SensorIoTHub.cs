using MakerDen.Json;
using MakerDen.Sensor;
using System;

namespace MakerDen.Telemetry
{

    public sealed class SensorIoTHub : ISensorTelemetry
    {
        JSONWriter data = new JSONWriter();

        public ISensor Sensor { get; }
        IComponent component;
        public string Channel => string.Empty;

        #region Device Properties
        string DeviceId { get; set; }

        #endregion

        /// <summary>
        /// Azure IoT Suite Simulated Device schema
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="measureName"></param>
        /// <param name="sensorUnit"></param>
        public SensorIoTHub(ISensor sensor, string deviceID)
        {

            this.Sensor = sensor;
            this.component = sensor as IComponent;
            // Only Device ID is not hard-coded, the rest of the Device Properties are hard-coded below, and the DeviceInfo message is sent to the IoT Hub to update 
            var loc = ConfigurationManager.DeviceLocations.Find(x => x.UniqueRPiDeviceID.Contains(deviceID.ToUpperInvariant()));
            if (loc == null)
            {
                // just defaults to 1 Epping Road
                loc = new ConfigurationManager.DeviceLocation("F4D9C1B3-F68C-47B6-8894-855E3D78CF4C", "rpi01", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi01;SharedAccessKey=JvGy9y1z6GlZFywrdAbFGw==", -33.796875, 151.138428);  // Microsoft Sydney, 1 Epping Road, North Ryde, NSW 2113, Australia
            }
            this.DeviceId = loc.DeviceID;
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
            // The pre-configured remote monitoring solution only recognizes telemetry such as humidity and temperature
            // Hence if there is no temperature reading, skip message sending altogether. Otherwise too many null values in the message would mess up the chart in the dashboard
            if (component.Type.Equals("temp"))
            {
                // Generate random humidity value between 50 - 55
                Random r = new Random();
                double randomHumidity = r.NextDouble() * 5 + 50;

                data.Begin();
                data.AddProperty("DeviceId", DeviceId);

                data.AddProperty("Temperature", Sensor.Value[0], 2);
                data.AddProperty("Humidity", randomHumidity, 2);

                data.AddProperty("ExternalTemperature", "null");
                data.End();

            }

        }
    }
}

