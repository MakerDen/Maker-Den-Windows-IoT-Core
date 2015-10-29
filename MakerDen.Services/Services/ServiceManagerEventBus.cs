using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Web.Http;

namespace MakerDen.Services
{
    public class ServiceManagerEventBus : IServiceManager
    {

        object publishLock = new object();

        string ServicebusNamespace = ConfigurationManager.ServicebusNamespace;
        string EventHubName = ConfigurationManager.EventHubName;
        string DeviceName = ConfigurationManager.DeviceId;
        string KeyName = ConfigurationManager.KeyName;
        string Key = ConfigurationManager.Key;

        int id = 0;

        HttpClient httpClient = new HttpClient();
        Uri uri;
        private string sas;

        public ServiceManagerEventBus()
        {
            InitEventHubConnection();
        }


        public uint Publish(string topic, byte[] data)
        {
            lock (publishLock)
            {
                sendMessage(Encoding.UTF8.GetString(data.ToArray(), 0, (int)data.Length)).Wait();
            }
            return 0;
        }

        private async Task sendMessage(string message)
        {
            id++;
            try
            {
                HttpStringContent content = new HttpStringContent(message, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
                HttpResponseMessage postResult = await httpClient.PostAsync(uri, content);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception when sending message:" + e.Message);
            }
        }

        private bool InitEventHubConnection()
        {
            try
            {
                this.uri = new Uri("https://" + ServicebusNamespace +
                              ".servicebus.windows.net/" + EventHubName +
                              //"/publishers/" + DeviceName +
                              "/messages");
                this.sas = SASTokenHelper();

                httpClient.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("SharedAccessSignature", sas);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string SASTokenHelper()
        {
            int expiry = (int)DateTime.UtcNow.AddDays(20).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            string stringToSign = UrlEncode(this.uri.ToString()) + "\n" + expiry.ToString();
            string signature = HmacSha256(Key, stringToSign);
            string token = String.Format("sr={0}&sig={1}&se={2}&skn={3}", UrlEncode(this.uri.ToString()), UrlEncode(signature), expiry, KeyName);

            return token;
        }

        /// <summary>
        /// Because Windows.Security.Cryptography.Core.MacAlgorithmNames.HmacSha256 doesn't exist in UWP
        public string HmacSha256(string key, string kvalue)
        {
            var keyStrm = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            var valueStrm = CryptographicBuffer.ConvertStringToBinary(kvalue, BinaryStringEncoding.Utf8);

            var objMacProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
            var hash = objMacProv.CreateHash(keyStrm);
            hash.Append(valueStrm);

            return CryptographicBuffer.EncodeToBase64String(hash.GetValueAndReset());
        }

        string UrlEncode(string value)
        {
            return Uri.EscapeDataString(value).Replace("%20", "+");
        }
    }
}
