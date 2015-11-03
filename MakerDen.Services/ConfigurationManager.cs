
using System.Collections.Generic;

namespace MakerDen
{
    public static class ConfigurationManager
    {
        public enum CloudMode
        {
            MQTT,
            EventHub,
            IoT_Hub,
            None
        }


        #region MQTT Config

        public static CloudMode CloudConnection { get; set; } = CloudMode.None;
        public static string Organisation { get; set; } = "Microsoft";
        public static string Location { get; set; } = "Melbourne";
        public static string NetworkId { get; set; }
        public static string DeviceId { get; set; }

        #endregion MQTT Config


        #region Event Hub Config

        public static string ServicebusNamespace { get; set; } = "MakerDen-ns";
        public static string EventHubName { get; set; } = "ehdevices";
        public static string KeyName { get; set; } = "D1";
        public static string Key { get; set; } = "sFhEe2pLQkWuzXW+5dcOmRZ36GrZy/9/io7DijcVhdc=";

        #endregion


        #region MQTT

        // Best efforts to run the MQTT Broker at gloveboxAE.cloudapp.net 
        // You can install your own instance of Mosquitto MQTT Server from http://mosquitto.org 
        public static string Broker { get; set; } = "gloveboxAE.cloudapp.net";
        public static string MqttNameSpace { get; set; } = "gb/";
        public static string[] MqqtSubscribe { get; set; } = new string[] { "gbcmd/#" };
        public static string MqttDeviceAnnounce { get; set; } = "gbdevice/";


        #endregion


        #region Azure IoT Hub Config
        public class DeviceLocation
        {
            public string DeviceID { get; set; }
            public string IoTHubConnectionString { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public DeviceLocation(string deviceID, string iotHub, double latitude, double longitude)
            {
                DeviceID = deviceID;
                IoTHubConnectionString = iotHub;
                Latitude = latitude;
                Longitude = longitude;
            }

        }

        public static List<DeviceLocation> DeviceLocations = new List<DeviceLocation>{
            new DeviceLocation("rpi01", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi01;SharedAccessKey=JvGy9y1z6GlZFywrdAbFGw==", -33.796875, 151.138428),  // Microsoft Sydney, 1 Epping Road, North Ryde, NSW 2113, Australia
            new DeviceLocation("rpi02", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi02;SharedAccessKey=oK8C0JHjKj9m4JN1KGQLEw==", -37.821998, 144.962951), // Microsoft Melbourne, Level 5, 4 Freshwater Place, Southbank, Victoria, Australia
            new DeviceLocation("rpi03", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi03;SharedAccessKey=rDdPuEicPo5fVLnvkx63Fg==", -27.468469, 153.020645), // Microsoft Brisbane, Level 28, 400 George Street, Brisbane, Queensland, Australia
            new DeviceLocation("rpi04", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi04;SharedAccessKey=RZP1UV6kR3YMOBA4YoFQcw==", -34.925518, 138.60463), // Microsoft Adelaide, Level 12, Aurora Building, 147 Pirie Street, Adelaide, South Australia, Australia
            new DeviceLocation("rpi05", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi05;SharedAccessKey=i15D5t8ErTR+OlA6iSOZOg==", -31.95449, 115.857803), // Microsoft Perth,  Level 3, 100 St Georges Terrace, Perth Western Australia 6000
            new DeviceLocation("rpi06", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi06;SharedAccessKey=sSI90Wx9UvMgfn9oEBKnjA==", -42.88174, 147.331054), // Microsoft Tasmania, 85 Macquarie Street, Hobart Tasmania 7000 
            new DeviceLocation("rpi07", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi07;SharedAccessKey=w1xLbI98r43jcaY6xDNpmw==", -35.306709, 149.13385), // Microsoft Canberra, Level 4, 6 National Circuit, Barton, Canberra Australian Capital Territory 2600
            new DeviceLocation("rpi08", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi08;SharedAccessKey=x4Ydwc+EpbHe7YjfxZK4qg==", -21.896395, 151.085846), // Great Barrier Reef, QLD
            new DeviceLocation("rpi09", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi09;SharedAccessKey=WDgf1OYTlkluW0zfYSZxpw==", -25.34469, 131.037994), // Uluru
            new DeviceLocation("rpi10", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi10;SharedAccessKey=UYpFCtybC1tsDD7W73/e4A==", -33.579071, 150.319397) // Blue Mountains NAtional Park, NSW
        };
        #endregion
    }
}
