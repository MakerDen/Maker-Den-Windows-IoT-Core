using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using System.Runtime.InteropServices.WindowsRuntime;

namespace MakerDen.Services
{
    public class ServiceManagerIoTHub : IServiceManager
    {
        object publishLock = new object();
        int id = 0;
        string IoTConnectionString;
        HttpClient httpClient;
        Uri uri;
        private string sas;
        private string DeviceID;
        private double Latitude;
        private double Longitude;

        const char Base64Padding = '=';
        static readonly HashSet<char> base64Table = new HashSet<char>{'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O',
                                                                      'P','Q','R','S','T','U','V','W','X','Y','Z','a','b','c','d',
                                                                      'e','f','g','h','i','j','k','l','m','n','o','p','q','r','s',
                                                                      't','u','v','w','x','y','z','0','1','2','3','4','5','6','7',
                                                                      '8','9','+','/' };


        public ServiceManagerIoTHub()
        {
        }

        public ServiceManagerIoTHub(string deviceID)
        {
            // Only Device ID is not hard-coded, the rest of the Device Properties are hard-coded below, and the DeviceInfo message is sent to the IoT Hub to update 
            var loc = ConfigurationManager.DeviceLocations.Find(x => x.UniqueRPiDeviceID.Contains(deviceID.ToUpperInvariant()));
            if (loc == null)
            {
                // just defaults to 1 Epping Road
                loc = new ConfigurationManager.DeviceLocation("F4D9C1B3-F68C-47B6-8894-855E3D78CF4C", "rpi01", "HostName=FaisterRemote.azure-devices.net;DeviceId=rpi01;SharedAccessKey=JvGy9y1z6GlZFywrdAbFGw==", -33.796875, 151.138428);  // Microsoft Sydney, 1 Epping Road, North Ryde, NSW 2113, Australia
            }
            this.IoTConnectionString = loc.IoTHubConnectionString;
            this.Latitude = loc.Latitude;
            this.Longitude = loc.Longitude;

            // Only HTTP/1 is supported in UWP apps currently
            //deviceClient = DeviceClient.CreateFromConnectionString(IoTConnectionString, TransportType.Http1);

            InitIoTHubConnection();
            UpdateDeviceInfo();
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
            if (String.IsNullOrEmpty(message))
                return;

            using (var msg = new HttpRequestMessage(HttpMethod.Post, this.uri))
            {
                if (this.uri == null) return;
                HttpResponseMessage responseMsg;
                try
                {
                    id++;
                    msg.Content = new StringContent(message);
                    msg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    responseMsg = await this.httpClient.SendAsync(msg);
                    if (responseMsg == null)
                    {
                        throw new InvalidOperationException("The response message was null when executing operation POST telemetry to IoT Hub");
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception when sending message:" + e.Message);
                }
            }
        }

        private bool InitIoTHubConnection()
        {
            try
            {
                string hostName = this.GetValue("HostName");
                this.DeviceID = this.GetValue("DeviceId");
                string sharedAccessKey = this.GetValue("SharedAccessKey");

                // MANDATORY to have ?api-version=2015-08-15-preview as the query string otherwise the IoT Hub HTTPS D2C endpoint would throw a HTTP error with StatusCode: 400, ReasonPhrase: 'Bad Request'
                string requestUri = String.Format("/devices/{0}/messages/events?api-version=2015-08-15-preview", DeviceID);
                string sr = String.Format("{0}/devices/{1}", hostName, DeviceID);

                // Note: The SAS Token is set to expire after 5 minutes so as to limit the telemetry being sent to the IoT Hub
                this.sas = BuildSignature(null, sharedAccessKey, sr, TimeSpan.FromMinutes(5));
                this.uri = new Uri(String.Format("https://{0}{1}", hostName, requestUri));

                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", sas);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void UpdateDeviceInfo()
        {
            try
            {
                // The following code is needed when this Device wakes up and send its updated device properties to the IoT Hub
                string createdDateTime = Util.CorrectedUtcTime.ToString("o");
                string deviceInfo = "{\"DeviceProperties\":{\"DeviceID\":\"" + this.DeviceID + "\",\"HubEnabledState\":true,\"CreatedTime\":\"" + createdDateTime + "\",\"DeviceState\":\"normal\",\"UpdatedTime\":null,\"Manufacturer\":\"Raspberry Pi Foundation\",\"ModelNumber\":\"RPi 2 Model B\",\"SerialNumber\":\"n/a\",\"FirmwareVersion\":\"10.0.10556\",\"Platform\":\"Windows 10 IoT Core\",\"Processor\":\"ARM Cortex-A7 CPU\",\"InstalledRAM\":\"1 GB\",\"Latitude\":" + this.Latitude + ",\"Longitude\":" + this.Longitude + "},\"Commands\":[{\"Name\":\"PingDevice\",\"Parameters\":null},{\"Name\":\"StartTelemetry\",\"Parameters\":null},{\"Name\":\"StopTelemetry\",\"Parameters\":null},{\"Name\":\"ChangeSetPointTemp\",\"Parameters\":[{\"Name\":\"SetPointTemp\",\"Type\":\"double\"}]},{\"Name\":\"DiagnosticTelemetry\",\"Parameters\":[{\"Name\":\"Active\",\"Type\":\"boolean\"}]},{\"Name\":\"ChangeDeviceState\",\"Parameters\":[{\"Name\":\"DeviceState\",\"Type\":\"string\"}]}],\"CommandHistory\":[],\"IsSimulatedDevice\":false,\"Version\":\"1.0\",\"ObjectType\":\"DeviceInfo\"}";

                this.Publish("DeviceInfo", Encoding.UTF8.GetBytes(deviceInfo));
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception when sending Device Info to IoT Hub:" + e.Message);
            }
        }

        private string GetValue(string key)
        {
            string[] keyValuePairs = IoTConnectionString.Split(';');
            string value;
            int strNumber;
            int strIndex = 0;
            for (strNumber = 0; strNumber < keyValuePairs.Length; strNumber++)
            {
                strIndex = keyValuePairs[strNumber].IndexOf(key);
                if (strIndex >= 0)
                    break;
            }
            if (key.Equals("SharedAccessKey"))
            {
                // special processing for SharedAccessKey because we can't just split it by '='. The key itself may contain numerous '='
                value = keyValuePairs[strNumber].Substring(16);
            }
            else
            {
                string[] keypairs = keyValuePairs[strNumber].Split('=');
                value = keypairs[1];
            }
            return value;

        }

        static string BuildSignature(string keyName, string key, string target, TimeSpan timeToLive)
        {
            string expiresOn = BuildExpiresOn(timeToLive);
            string audience = WebUtility.UrlEncode(target);
            List<string> fields = new List<string>();
            fields.Add(audience);
            fields.Add(expiresOn);

            // Example string to be signed:
            // dh://myiothub.azure-devices.net/a/b/c?myvalue1=a
            // <Value for ExpiresOn>

            string signature = Sign(string.Join("\n", fields), key);

            // Example returned string:
            // SharedAccessSignature sr=ENCODED(dh://myiothub.azure-devices.net/a/b/c?myvalue1=a)&sig=<Signature>&se=<ExpiresOnValue>[&skn=<KeyName>]

            var buffer = new StringBuilder();
            buffer.AppendFormat(CultureInfo.InvariantCulture, "sr={0}&sig={1}&se={2}",
                                audience,
                                WebUtility.UrlEncode(signature),
                                WebUtility.UrlEncode(expiresOn));

            if (!string.IsNullOrEmpty(keyName))
            {
                buffer.AppendFormat(CultureInfo.InvariantCulture, "&{0}={1}",
                    "skn", WebUtility.UrlEncode(keyName));
            }

            return buffer.ToString();
        }

        static string BuildExpiresOn(TimeSpan timeToLive)
        {
            DateTime expiresOn = Util.CorrectedUtcTime.Add(timeToLive);
            TimeSpan secondsFromBaseTime = expiresOn.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            long seconds = Convert.ToInt64(secondsFromBaseTime.TotalSeconds, CultureInfo.InvariantCulture);
            return Convert.ToString(seconds, CultureInfo.InvariantCulture);
        }


        static string Sign(string requestString, string key)
        {
            if (!IsBase64String(key))
                throw new ArgumentException("The SharedAccessKey of the device is not a Base64String");

            var algo = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
            var keyMaterial = Convert.FromBase64String(key).AsBuffer();
            var hash = algo.CreateHash(keyMaterial);
            hash.Append(CryptographicBuffer.ConvertStringToBinary(requestString, BinaryStringEncoding.Utf8));

            var sign = CryptographicBuffer.EncodeToBase64String(hash.GetValueAndReset());
            return sign;
        }

        public static bool IsBase64String(string value)
        {
            value = value.Replace("\r", string.Empty).Replace("\n", string.Empty);

            if (value.Length == 0 || (value.Length % 4) != 0)
            {
                return false;
            }

            var lengthNoPadding = value.Length;
            value = value.TrimEnd(Base64Padding);
            var lengthPadding = value.Length;

            if ((lengthNoPadding - lengthPadding) > 2)
            {
                return false;
            }

            foreach (char c in value)
            {
                if (!base64Table.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
