
using MakerDen.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.System.Threading;

namespace MakerDen
{
    public static class Util
    {

        //static ThreadPoolTimer ntpTimer;
        //static NtpClient ntp = new NtpClient();
        static bool NtpInitalised = false;

        private readonly static string[] postcodes = new string[] { "3000", "6000", "2011" };
        private static Random rnd = new Random(Environment.TickCount);

        private static TimeSpan utcOffset;

        public static DateTime CorrectedUtcTime => utcOffset == null ? DateTime.UtcNow : DateTime.UtcNow - Util.utcOffset;

        public static async void StartNtpTimeService()
        {
            try
            {
                if (NtpInitalised) { return; }
                NtpInitalised = true;

                NtpClient ntp = new NtpClient("au.pool.ntp.org");

                //http://stackoverflow.com/questions/1193955/how-to-query-an-ntp-server-using-c
                var accurateTime = await ntp.GetNetworkTimeAsync(TimeSpan.FromSeconds(15));

                utcOffset = DateTime.UtcNow.Subtract(accurateTime);
            }
            catch { }

        //    ntpTimer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromMinutes(5));
        }


        private static async void Timer_Tick(ThreadPoolTimer timer)
        {
            await Task.Yield();
            try
            {
                NtpClient ntp = new NtpClient("au.pool.ntp.org");

                var accurateTime = await ntp.GetNetworkTimeAsync(TimeSpan.FromSeconds(10));

                utcOffset = DateTime.UtcNow.Subtract(accurateTime);
            }
            catch { }
        }

        public static string GetHostName()
        {
            var hostNamesList = Windows.Networking.Connectivity.NetworkInformation
                .GetHostNames();

            foreach (var entry in hostNamesList)
            {
                if (entry.Type == Windows.Networking.HostNameType.DomainName)
                {
                    return entry.CanonicalName.Split('.')[0];
                }
            }

            return null;
        }


        public static string RandomPostcode()
        {
            return postcodes[rnd.Next(postcodes.Length)];
        }

        public static string BytesToString(byte[] Input)
        {
            char[] Output = new char[Input.Length];
            for (int Counter = 0; Counter < Input.Length; ++Counter)
            {
                Output[Counter] = (char)Input[Counter];
            }
            return new string(Output);
        }

        public static void SetName(string networkId)
        {
            ConfigurationManager.NetworkId = networkId;
            //ConfigurationManager.Location = networkId;
        }

        static public IServiceManager StartNetworkServices(bool connected)
        {
            ConfigurationManager.DeviceId = GetHostName();

            if (!connected) { return null; }

            switch (ConfigurationManager.CloudConnection)
            {
                case ConfigurationManager.CloudMode.MQTT:
                    return new ServiceManagerMqtt(ConfigurationManager.Broker, connected);
                case ConfigurationManager.CloudMode.EventHub:
                    return new ServiceManagerEventBus();
                case ConfigurationManager.CloudMode.IoT_Hub:
                    return new ServiceManagerIoTHub(ConfigurationManager.DeviceId);
                default:
                    break;
            }

            return null;
        }

        // pull back the ethernet guid
        public static string UniqueDeviceId => NetworkInformation.GetConnectionProfiles().FirstOrDefault().NetworkAdapter.NetworkAdapterId.ToString();


        private static string MacToString(byte[] macAddress)
        {
            string result = string.Empty;
            foreach (var part in macAddress)
            {
                result += part.ToString("X") + "-";
            }
            return result.Substring(0, result.Length - 1);
        }

        public static void Delay(int milliseconds)
        {
            Task.Delay(milliseconds).Wait();
        }

        public static string GetIPAddress()
        {
            string localIP = "?";

            HostName localHostName = NetworkInformation.GetHostNames().FirstOrDefault(h =>
                    h.IPInformation != null &&
                    h.IPInformation.NetworkAdapter != null);

            localIP = localHostName.RawName;

            return localIP;
        }
    }
}
