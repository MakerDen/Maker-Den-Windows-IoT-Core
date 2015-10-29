using MakerDen.Json;
using MakerDen.Sensor;
using Microsoft.Azure.Devices.Client;
using System;
using System.Diagnostics;
using System.Text;

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
            var loc = ConfigurationManager.DeviceLocations.Find(x => x.DeviceID.Contains(deviceID));
            if (loc == null)
            {
                // just defaults to 1 Epping Road
                loc = new ConfigurationManager.DeviceLocation("rpi01", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi01;SharedAccessKey=JvGy9y1z6GlZFywrdAbFGw==", -33.796875, 151.138428);  // Microsoft Sydney, 1 Epping Road, North Ryde, NSW 2113, Australia
            }
            string iotConnectionString = loc.IoTHubConnectionString;
            try
            {
                DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(iotConnectionString, TransportType.Http1);
                // The following code is needed when this Device wakes up and send its updated device properties to the IoT Hub
                string createdDateTime = Util.CorrectedUtcTime.ToString("o");
                string deviceInfo = "{\"DeviceProperties\":{\"DeviceID\":\"" + deviceID + "\",\"HubEnabledState\":true,\"CreatedTime\":\"" + createdDateTime + "\",\"DeviceState\":\"normal\",\"UpdatedTime\":null,\"Manufacturer\":\"Raspberry Pi Foundation\",\"ModelNumber\":\"RPi 2 Model B\",\"SerialNumber\":\"n/a\",\"FirmwareVersion\":\"10.0.10556\",\"Platform\":\"Windows 10 IoT Core\",\"Processor\":\"900MHz quad-core ARM Cortex-A7 CPU\",\"InstalledRAM\":\"1 GB\",\"Latitude\":" + loc.Latitude + ",\"Longitude\":" + loc.Longitude + "},\"Commands\":[{\"Name\":\"PingDevice\",\"Parameters\":null},{\"Name\":\"StartTelemetry\",\"Parameters\":null},{\"Name\":\"StopTelemetry\",\"Parameters\":null},{\"Name\":\"ChangeSetPointTemp\",\"Parameters\":[{\"Name\":\"SetPointTemp\",\"Type\":\"double\"}]},{\"Name\":\"DiagnosticTelemetry\",\"Parameters\":[{\"Name\":\"Active\",\"Type\":\"boolean\"}]},{\"Name\":\"ChangeDeviceState\",\"Parameters\":[{\"Name\":\"DeviceState\",\"Type\":\"string\"}]}],\"CommandHistory\":[],\"IsSimulatedDevice\":true,\"Version\":\"1.0\",\"ObjectType\":\"DeviceInfo\"}";

                var devPropMsg = new Message(Encoding.UTF8.GetBytes(deviceInfo));


                // added the var result to suppress the warning - dglover
                var result = deviceClient.SendEventAsync(devPropMsg);

            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception when sending message:" + e.Message);
            }


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

            // Generate random humidity value between 50 - 55
            Random r = new Random();
            double randomHumidity = r.NextDouble() * 5 + 50;

            data.Begin();
            data.AddProperty("DeviceId", DeviceId);

            if (component.Type.Equals("temp"))
            {
                data.AddProperty("Temperature", Sensor.Value[0], 2);
                data.AddProperty("Humidity", randomHumidity, 2);
            }

            data.AddProperty("ExternalTemperature", "null");
            data.End();
        }
    }
}

