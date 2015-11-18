using MakerDen.Command;
using MakerDen.Json;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace MakerDen.Services
{
    public class ServiceManagerMqtt : IServiceManager
    {
        const int networkSettleTime = 6000;
        uint errorCount;
        bool networkChanged = false;
        bool networkAvailable = true;
        bool connected;
        string serviceAddress;
        public MqttClient mqtt = null;
        readonly string clientId;
        readonly string uniqueDeviceIdentifier;
        int lastSystemrequest = Environment.TickCount;
        DateTime lastSystemRequestTime = DateTime.Now;
        private object publishLock = new object();

        public ServiceManagerMqtt(string serviceAddress, bool connected)
        {

            string mqttError = string.Empty;
            this.serviceAddress = serviceAddress;
            this.connected = connected;
            this.clientId = CreateClientId();
            this.uniqueDeviceIdentifier = GetUniqueDeviceIdentifier(ConfigurationManager.NetworkId);

            Initialise();


        }

        public async void Initialise()
        {
            uint retryCount = 0;
            string mqttError = string.Empty;
        //    const uint MaxRetryCount = 1;

            await Task.Yield();

            if (!connected) { return; }

            while (mqtt == null) // && retryCount < MaxRetryCount)
            {
                try { StartMqtt(); }
                catch (Exception ex)
                {
                    retryCount++;
                    mqttError = ex.Message;
                    if (retryCount < 10) { await Task.Delay(2000); } else { await Task.Delay(10000); }
                }
            }
        }


        private string CreateClientId()
        {
            DateTime utc = DateTime.UtcNow;
            string cid = utc.Hour.ToString() + utc.Minute.ToString() + utc.Second.ToString() + "-" + Util.UniqueDeviceId;
            return cid.Length > 23 ? cid.Substring(0, 23) : cid;  //23 chars for clientid is mqtt max allowed
        }

        private string GetUniqueDeviceIdentifier(string value)
        {
            string id = string.Empty;

            if (value == null || value == string.Empty)
            {
                id = Util.UniqueDeviceId;
                if (id == null || id == string.Empty)
                {
                    id = Guid.NewGuid().ToString();
                }
            }
            else
            {
                Regex r = new Regex("/");
                id = r.Replace(value, "");
            }
            return id;
        }


        void StartMqtt()
        {

            bool networkReset = false;
            if (!connected) { return; }

            // give the network some settle time
            Util.Delay(networkSettleTime);

            while (mqtt == null || !mqtt.IsConnected)
            {
                networkReset = false;
                while (!networkAvailable)
                {
                    Task.Delay(2000).Wait();
                    networkReset = true;
                }
                // the network needs a bit of settle time, dhcp etc
                if (networkReset) { Util.Delay(networkSettleTime); }

                mqtt = new MqttClient(serviceAddress);
                if (mqtt != null && networkAvailable) { mqtt.Connect(clientId); }

            }

            mqtt.MqttMsgPublishReceived += mqtt_MqttMsgPublishReceived;
            mqtt.Subscribe(ConfigurationManager.MqqtSubscribe, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        void ReconnectMqtt()
        {
            ResetMqtt();
            StartMqtt();
        }

        void ResetMqtt()
        {
            if (mqtt != null)
            {
                mqtt.MqttMsgPublishReceived -= mqtt_MqttMsgPublishReceived;
                mqtt = null;
            }
        }

        void mqtt_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            DateTime now = DateTime.Now;

            if (now < lastSystemRequestTime.AddSeconds(2) || e.Message.Length > 4096 || e.Topic.Length > 256) { return; }

            lastSystemRequestTime = now;

            var actionRequest = DecodeAction(e.Topic, Util.BytesToString(e.Message));
            if (actionRequest == null) { return; }

            string[] result = IotActionManager.Action(actionRequest);
            if (result != null)
            {
                Publish(ConfigurationManager.MqttDeviceAnnounce + ConfigurationManager.DeviceId, SystemConfig(result));
            }
        }

        private byte[] SystemConfig(string[] IotItems)
        {
            JSONWriter jw = new JSONWriter();
            jw.Begin();
            jw.AddProperty("Dev", ConfigurationManager.DeviceId);
            jw.AddProperty("Id", uniqueDeviceIdentifier);
            jw.AddProperty("Items", IotItems);
            jw.End();

            return jw.ToArray();
        }

        private IotAction DecodeAction(string topic, string message)
        {
            string[] topicParts = topic.ToLower().Split('/');

            if (topic.Length < 9) { return null; }

            switch (topic.Substring(0, 9))
            {
                case "gbcmd/all":
                    return ActionParts(topicParts, 2, message);
                case "gbcmd/dev":
                    // check device guid matches requested
                    if (topicParts.Length > 2 && topicParts[2] != string.Empty && topicParts[2] != null && topicParts[2] == uniqueDeviceIdentifier.ToLower())
                    {
                        return ActionParts(topicParts, 3, message);
                    }
                    else { return null; }
            }
            return null;
        }

        private IotAction ActionParts(string[] topicParts, int startPos, string message)
        {
            IotAction action = new IotAction();
            action.parameters = message;

            for (int i = startPos, p = 0; i < topicParts.Length; i++, p++)
            {
                string part = topicParts[i].Length == 0 ? null : topicParts[i];
                if (part == null) { continue; }
                switch (p)
                {
                    case 0:
                        action.cmd = part;
                        break;
                    case 1:
                        action.item = part;
                        break;
                    case 2:
                        action.subItem = part;
                        break;
                    default:
                        break;
                }
            }
            return action;
        }

        private void PublishData(string topic, byte[] data)
        {
            try
            {
                while (mqtt == null || !mqtt.IsConnected || networkChanged)
                {
                    networkChanged = false;
                    ReconnectMqtt();
                }
                mqtt.Publish(topic, data);
            }
            catch (Exception)
            {
                networkChanged = true;
                errorCount++;
            }
        }

        public uint Publish(string topic, byte[] data)
        {
            lock (publishLock)
            {
                if (!connected) { return 0; }

                PublishData(topic, data);

                return errorCount;
            }
        }
    }
}
