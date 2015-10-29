using Glovebox.Graphics.Components;
using Glovebox.IoT.Devices.Actuators;
using Glovebox.IoT.Devices.Converters;
using MakerDen.Sensor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Adc;
using Windows.Devices.Adc.Provider;

namespace MakerDen.Components.HATs
{
    public class ProtoShield
    {
        AdcProviderManager adcManager;
        public IReadOnlyList<AdcController> adcControllers;


        public IAdcControllerProvider Adc { get; set; }

        //public BMP280Service bmp280 { get; set; }
        public LED8x8Matrix matrix { get; set; }

        public Led[] Led { get; set; }

        public BMP280Sensor bmp280TempPressure { get; set; }

        public async Task InitaliseAdc()
        {
            if (Adc == null) { return; }

            adcManager = new AdcProviderManager();
            adcManager.Providers.Add(new MCP3002());
            adcControllers = await adcManager.GetControllersAsync();
        }


        public void InitialiseHat()
        {
            InitaliseAdc().Wait();
        }
    }
}
