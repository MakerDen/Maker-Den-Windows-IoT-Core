using Glovebox.Graphics.Components;
using Glovebox.Graphics.Drivers;
using Glovebox.Graphics.Grid;
using Glovebox.IoT.Devices.Converters;
using Glovebox.IoT.Devices.Sensors;
using MakerDen.Components.HATs;
using MakerDen.Sensor;
using MakerDen.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static MakerDen.ConfigurationManager;

namespace MakerDen
{
    public class WeatherHatServices : ProtoShield
    {
        private IServiceManager sm;

        public void Initialise(string name, CloudMode cloudMode = CloudMode.None)
        {
            Adc = new MCP3002();
            matrix = new LED8x8Matrix(new Ht16K33());
            bmp280TempPressure = new BMP280Sensor() { SampleRateMilliseconds = 30000 };

            InitialiseHat();

            Util.SetName(name);
            SensorMgr.DeviceName = Util.GetHostName();

            StartNetworkServices(cloudMode);
        }

        protected void StartNetworkServices(CloudMode cloudMode = CloudMode.MQTT)
        {
            if (cloudMode == CloudMode.None) { return; }

            ConfigurationManager.CloudConnection = cloudMode;
            bool connected = true;
            Util.StartNtpTimeService();  // this is a hack workaround for system time sync issue on W10 for IoT
            sm = Util.StartNetworkServices(connected);
        }

        protected void Display(Grid8x8.Symbols sym)
        {
            if (matrix == null) { return; }

            matrix.DrawSymbol(sym);
            matrix.FrameDraw();
        }


        protected void Display(string text)
        {
            if (matrix == null) { return; }

            matrix.FrameClear();
            matrix.ScrollStringInFromRight(text, 100);
        }

        public async Task DisplayBmp280(ISensor sensor)
        {
            var component = sensor as BMP280;

            if (matrix == null) { return; }
            while (true)
            {
                var message = $"{Math.Round(component.Temperature.DegreesCelsius, 1)}C , {Math.Round(component.Pressure.Hectopascals, 1)}hPa";
                Display(message);  // Display temp on matrix
                await Task.Delay(1000);
            }
        }

        public uint OnBeforeMeasure(object sender, EventArgs e)
        {
            int id = ((SensorMgr.SensorIdEventArgs)e).id;
            if (Led == null) { return 0; }
            FlashLed(id);
            return 0;
        }

        async void FlashLed(int id)
        {
            Led[id % Led.Length].On();
            await Task.Delay(250);
            Led[id % Led.Length].Off();
        }


        public uint SetLEDMatrixBrightness(object sender, EventArgs e)
        {
            if (matrix == null) { return 0; }

            var sensorTelemetry = ((SensorMgr.SensorItemEventArgs)e).SensorTelemetry;

            int lvl = (int)(sensorTelemetry.Sensor.Value[0] % 100) / 30;
            matrix.SetBrightness((byte)lvl);

            return 0;
        }

        protected uint OnAfterMeasurement(object sender, EventArgs e)
        {
            var sensorTelemetry = ((SensorMgr.SensorItemEventArgs)e).SensorTelemetry;
            var json = sensorTelemetry.ToJson();


            if (sm == null)
            {
                Debug.WriteLine(sensorTelemetry.ToString());
                return 0;
            }

            var topic = sensorTelemetry.Channel;

            return sm.Publish(topic, json);
        }
    }
}
