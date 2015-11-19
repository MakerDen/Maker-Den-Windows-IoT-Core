
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
        public static string Location { get; set; } = "Sydney";
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
            public string UniqueRPiDeviceID { get; set; }
            public string DeviceID { get; set; }
            public string IoTHubConnectionString { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public DeviceLocation(string uniqueID, string deviceID, string iotHub, double latitude, double longitude)
            {
                UniqueRPiDeviceID = uniqueID;
                DeviceID = deviceID;
                IoTHubConnectionString = iotHub;
                Latitude = latitude;
                Longitude = longitude;
            }

        }

        public static List<DeviceLocation> DeviceLocations = new List<DeviceLocation>{
            new DeviceLocation("F4D9C1B3-F68C-47B6-8894-855E3D78CF4C", "rpi01", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi01;SharedAccessKey=JvGy9y1z6GlZFywrdAbFGw==", -33.796875, 151.138428),  // Microsoft Sydney, 1 Epping Road, North Ryde, NSW 2113, Australia
            new DeviceLocation("F10E711A-16B4-4CF2-B224-2D0091164026", "rpi02", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi02;SharedAccessKey=oK8C0JHjKj9m4JN1KGQLEw==", -37.821998, 144.962951), // Microsoft Melbourne, Level 5, 4 Freshwater Place, Southbank, Victoria, Australia
            new DeviceLocation("BD9A56C0-0807-47A3-B9C2-96FA41BA796D", "rpi03", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi03;SharedAccessKey=rDdPuEicPo5fVLnvkx63Fg==", -27.468469, 153.020645), // Microsoft Brisbane, Level 28, 400 George Street, Brisbane, Queensland, Australia
            new DeviceLocation("5998800B-314F-41FA-A661-72339740EEC0", "rpi04", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi04;SharedAccessKey=RZP1UV6kR3YMOBA4YoFQcw==", -34.925518, 138.60463), // Microsoft Adelaide, Level 12, Aurora Building, 147 Pirie Street, Adelaide, South Australia, Australia
            new DeviceLocation("A85D5AB4-0AF3-4C5B-A328-58394779FD60", "rpi05", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi05;SharedAccessKey=i15D5t8ErTR+OlA6iSOZOg==", -31.95449, 115.857803), // Microsoft Perth,  Level 3, 100 St Georges Terrace, Perth Western Australia 6000
            new DeviceLocation("3AE1AAD9-2E3F-4DA0-8F6A-F1F25D95E970", "rpi06", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi06;SharedAccessKey=sSI90Wx9UvMgfn9oEBKnjA==", -42.88174, 147.331054), // Microsoft Tasmania, 85 Macquarie Street, Hobart Tasmania 7000 
            new DeviceLocation("76B7447E-9234-4FD6-BBCB-F788CEF8DF92", "rpi07", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi07;SharedAccessKey=w1xLbI98r43jcaY6xDNpmw==", -35.306709, 149.13385), // Microsoft Canberra, Level 4, 6 National Circuit, Barton, Canberra Australian Capital Territory 2600
            new DeviceLocation("83D87149-EF2A-406D-93B0-6DF8EC4EAFEC", "rpi08", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi08;SharedAccessKey=x4Ydwc+EpbHe7YjfxZK4qg==", -21.896395, 151.085846), // Great Barrier Reef, QLD
            new DeviceLocation("FA58FE68-83AA-41A5-A778-C9FD68DE8206", "rpi09", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi09;SharedAccessKey=WDgf1OYTlkluW0zfYSZxpw==", -25.34469, 131.037994), // Uluru
            new DeviceLocation("98C2D521-18FE-482C-AF24-22B6BF7C0BDE", "rpi10", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi10;SharedAccessKey=UYpFCtybC1tsDD7W73/e4A==", -33.579071, 150.319397), // Blue Mountains National Park, NSW
            new DeviceLocation("1544085A-42B4-4F91-92FB-8B55468F446F", "R2D2", "HostName=FaisterRemote.azure-devices.net;DeviceId=R2D2;SharedAccessKey=eZ8Q1omOZGHsXmt/R2aq5w==", -28.0266, 153.435608) // Broadbeach, Gold Coast, QLD
        };
        #endregion
    }
}
