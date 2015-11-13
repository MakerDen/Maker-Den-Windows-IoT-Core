using Glovebox.Graphics.Components;
using Glovebox.Graphics.Drivers;
using Glovebox.Graphics.Grid;
using Glovebox.IoT.Devices.Converters;
using Glovebox.IoT.Devices.HATs;
using MakerDen.Sensor;
using MakerDen.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Adc;
using static MakerDen.ConfigurationManager;

namespace MakerDen
{
    public class ExplorerHatProServices : ExplorerHatPro
    {

        AdcProviderManager adcManager = new AdcProviderManager();
        public AdcController ADS1015 { get; private set; }


        private IServiceManager sm;

        protected LightSensor light;
        protected TempSensor temp;
        protected MemSensor mem;


        protected LED8x8Matrix matrix;


        protected async void Initialise(string yourName = "Maker", CloudMode cloudMode = CloudMode.None)
        {
            Util.SetName(yourName);
            SensorMgr.DeviceName = Util.GetHostName();

            StartNetworkServices(cloudMode);

            InitialiseHatAsync(false, true, false).Wait();

            // for now not using the adc on the explorer hat as it's set to 3.3volts
            adcManager.Providers.Add(new ADS1015(Glovebox.IoT.Devices.Converters.ADS1015.Gain.Volt5));
            ADS1015 = (await adcManager.GetControllersAsync())[0];

            ShortLedShow();

            light = new LightSensor(ADS1015.OpenChannel((int)Pin.Analog.A3));
            temp = new TempSensor(ADS1015.OpenChannel((int)Pin.Analog.A4), 5000); //version one of wiring on Explorer Pro for ADC = 5v - this will change to 3.3v in the future 
            mem = new MemSensor();

            matrix = new LED8x8Matrix(new Ht16K33());
        }


        private void StartNetworkServices(CloudMode cloudMode)
        {

            if (cloudMode == CloudMode.None) { return; }

            ConfigurationManager.CloudConnection = cloudMode;
            bool connected = true;
            Util.StartNtpTimeService();  // this is a hack workaround for system time sync issue on W10 for IoT
            sm = Util.StartNetworkServices(connected);
        }


        public async Task DisplayTemperature()
        {
            if (matrix == null) { return; }

            matrix.DrawSymbol(Grid8x8.Symbols.HourGlass);
            matrix.FrameDraw();
            await Task.Delay(1500); // give network services time to initialise

            while (true)
            {
                if (temp != null)
                {
                    var message = $"{Math.Round(temp.Temperature.DegreesCelsius, 1)}C";
                    Display(message);  // Display temp on matrix
                }
                await Task.Delay(1000);
            }
        }


        private async void ShortLedShow()
        {
            for (int c = 0; c < 4; c++)
            {
                Leds[0].On();
                Leds[3].On();
                await Task.Delay(30);
                Leds[1].On();
                Leds[2].On();
                await Task.Delay(30);
                Leds[0].Off();
                Leds[3].Off();
                await Task.Delay(30);
                Leds[1].Off();
                Leds[2].Off();
                await Task.Delay(30);
            }
        }

        protected void Welcome()
        {
            matrix.ScrollStringInFromRight("Hello " + ConfigurationManager.NetworkId + " I'm " + Util.GetHostName().ToUpper() + " at " + Util.GetIPAddress() + " ", 100);
        }

        protected void Display(Grid8x8.Symbols sym)
        {
            matrix.DrawSymbol(sym);
            matrix.FrameDraw();
        }


        protected void Display(string text)
        {
            matrix.FrameClear();
            matrix.ScrollStringInFromRight(text, 100);
        }

        public uint OnBeforeMeasure(object sender, EventArgs e)
        {
            int id = ((SensorMgr.SensorIdEventArgs)e).id;
            if (Leds == null) { return 0; }
            FlashLed(id);
            return 0;
        }

        async void FlashLed(int id)
        {
            Leds[id % Leds.Length].On();
            await Task.Delay(250);
            Leds[id % Leds.Length].Off();
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

            if (sm == null)
            {
                Debug.WriteLine(sensorTelemetry.ToString());
                return 0;
            }

            var json = sensorTelemetry.ToJson();

            var topic = sensorTelemetry.Channel;

            return sm.Publish(topic, json);
        }
    }
}
