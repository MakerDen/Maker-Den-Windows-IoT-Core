using Microsoft.Azure.Devices.Client;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakerDen.Services
{
    public class ServiceManagerIoTHub : IServiceManager
    {

        object publishLock = new object();
        int id = 0;

        string IoTConnectionString;

        public ServiceManagerIoTHub()
        {

        }

        public ServiceManagerIoTHub(string deviceID)
        {
            // Only Device ID is not hard-coded, the rest of the Device Properties are hard-coded below, and the DeviceInfo message is sent to the IoT Hub to update 
            var loc = ConfigurationManager.DeviceLocations.Find(x => x.DeviceID.Contains(deviceID));
            if (loc == null)
            {
                // just defaults to 1 Epping Road
                loc = new ConfigurationManager.DeviceLocation("rpi01", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi01;SharedAccessKey=JvGy9y1z6GlZFywrdAbFGw==", -33.796875, 151.138428);  // Microsoft Sydney, 1 Epping Road, North Ryde, NSW 2113, Australia
            }
            IoTConnectionString = loc.IoTHubConnectionString;
        }


        public uint Publish(string topic, byte[] data)
        {
            lock (publishLock)
            {
                Task.Run(() => { sendMessage(Encoding.UTF8.GetString(data.ToArray(), 0, (int)data.Length)).Wait(); }).Wait();
            }
            return 0;
        }

        private async Task sendMessage(string message)
        {
            // The pre-configured remote monitoring solution only recognizes telemetry such as humidity and temperature
            // Hence if there is no temperature reading, skip message sending altogether. Otherwise too many null values in the message would mess up the chart in the dashboard
            if (message.IndexOf("Temperature") != -1)
            {

                id++;
                try
                {
                    DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(IoTConnectionString, TransportType.Http1);

                    var msg = new Message(Encoding.UTF8.GetBytes(message));

                    await deviceClient.SendEventAsync(msg);

                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception when sending message:" + e.Message);
                }
            }
        }

    }
}
