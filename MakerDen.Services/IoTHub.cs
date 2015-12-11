using MakerDen.Json;
using MakerDen.Sensor;
using MakerDen.Services;
using MakerDen.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakerDen
{
    public class IoTHub
    {
        ISensor[] sensors;
        JSONWriter data = new JSONWriter();
        ServiceManagerIoTHub iotHubService;

        int sampleRateMilliseconds;

        public IoTHub(ISensor[] sensors, int sampleRateMilliseconds = 2000)
        {
            this.sensors = sensors;
            this.sampleRateMilliseconds = sampleRateMilliseconds;
            StartMeasuringAsync();
           
        }

        private async void StartMeasuringAsync()
        {
            string deviceId = GetDeviceId();
            iotHubService = new ServiceManagerIoTHub(deviceId);

            while (true)
            {
                data.Begin();
                data.AddProperty("DeviceId", deviceId);
                data.AddProperty("Utc", Util.CorrectedUtcTime);

                foreach (var sensor in sensors)
                {
                    sensor.Measure();
                    for (int samples = 0; samples < (int)sensor.ValuesPerSample; samples++)
                    {
                        data.AddProperty(sensor.UnitofMeasure[samples], (float)sensor.Value[samples]);
                    }
                }

                data.End();

                Debug.WriteLine(data.ToString());
            //    iotHubService.Publish("", jsonData.ToArray());

                await Task.Delay(sampleRateMilliseconds);
            }
        }

        private string GetDeviceId()
        {
            var loc = ConfigurationManager.DeviceLocations.Find(x => x.UniqueRPiDeviceID.Contains(Util.UniqueDeviceId.ToUpperInvariant()));
            if (loc == null)
            {
                // just defaults to 1 Epping Road
                loc = new ConfigurationManager.DeviceLocation("F4D9C1B3-F68C-47B6-8894-855E3D78CF4C", "rpi01", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi01;SharedAccessKey=JvGy9y1z6GlZFywrdAbFGw==", -33.796875, 151.138428);  // Microsoft Sydney, 1 Epping Road, North Ryde, NSW 2113, Australia
            }
            return loc.DeviceID;
        }
    }
}
