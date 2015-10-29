# Internet of Things Maker Den on Windows 10 for IoT on Raspberry Pi 2

See the [Wiki](https://github.com/MakerDen/IoT-Maker-Den-Windows-for-IoT/wiki) for more information.


##Software Requirements (as at May 2015)

1. Windows 10 (build 10122) or above
2. [Visual Studio 2015](https://www.visualstudio.com/vs-2015-product-editions) (Community Edition (free), Professional or above)


##Getting Started
For information on setting up your Raspberry Pi 2 running Windows 10 for IoT and your development environment see http://ms-iot.github.io/content/GetStarted.htm.

##Acknowledgements
1. The Component library was inspired by the [Raspberry# IO](https://github.com/raspberry-sharp/raspberry-sharp-io) .NET/Mono IO Library for Raspberry Pi project 
2. The [MQTT Client Library for .Net and WinRT](https://m2mqtt.codeplex.com)

##Hardware

###Barebones
1. [Raspberry Pi 2](https://www.raspberrypi.org/). With no sensors or [Explorer HAT Pro](http://shop.pimoroni.com/products/explorer-hat) attached the Raspberry Pi can publish memory usage data enough to get you started and publishing data:)

###Ideal Hardware
This lab is built around the [Explorer HAT Pro](http://shop.pimoroni.com/products/explorer-hat) as it has a four channel ADC, capacitive touch pads, four coloured LEDs, two H-bridge motor drivers, 5V input and output.



1. [Raspberry Pi 2](https://www.raspberrypi.org/)
2. [Explorer Pro HAT](http://shop.pimoroni.com/products/explorer-hat)
3. Light Dependent Resistor
4. [Analogue Temperature Sensor](http://au.rs-online.com/web/p/temperature-humidity-sensors/0403838/)

###Optional Extras
5. [Adafruit Mini 8x8 LED Matrix](http://tronixlabs.com/display/led/matrix/adafruit-small-1-2-8x8-led-matrix-w-i2c-backpack-red/)
6. [Breakout Board for Electret Microphone](http://littlebirdelectronics.com.au/products/breakout-board-for-electret-microphone)


Explorer Pro HAT [Notes](https://github.com/pimoroni/explorer-hat):

1. Explorer HAT uses an output driver chip called the ULN2003A, which contains a set of transistor pairs called a Darlington Array. It transforms the small logic signal of the Pi into something capable of driving much bigger loads, such as motors, steppers, lights and more. 
2. The 4 outputs on Explorer HAT can sink 5V, but not source. This means you need to connect your load to one of the 5V pins, and then to the output. When you turn the output on it will connect your circuit to ground, allowing current to flow and your load to turn on. This is the opposite of using a bare Pi GPIO pin, where you might connect to the pin and then to ground; keep this in mind!


#What is The Internet of Things Maker Den?

The Internet of Things Maker Den is designed to be an accessible hands on experience with hardware prototyping and building a Universal Windows App that you will deploy to a 
Raspberry Pi 2 running Windows 10 for IoT..  

The goal of the lab is to learn something about wiring circuits (with plenty of guidance), deploying code and streaming your sensor data to Microsoft Azure.  

The Maker Den is implemented on top of the Internet of Things Solutions Framework.

#What is the Internet of Things Solution Framework?

The Internet of Things Solution Framework for Windows 10 on IoT is a general purpose extensible pluggable foundation to support sensors, actuators, data serialisation, communications,
and command and control.


![Alt text](https://github.com/MakerDen/IoT-Maker-Den-NETMF/blob/master/MakerDen/Lab%20Code/Maker%20Den%20IoT%20Framework.jpg)


## Extensible/pluggable framework supporting

1. Sensors
 * Physical: Light, Temperature
 * Virtual: Memory Usage, Diagnostics
 * Sensor data serialised to a JSON schema

2. Actuators
 * Led, Relay, Generalised Output Pin
 * Adafruit mini 8x8 LED Matrix on HT16K33 I2C Controller
    - Low and high level pixel frame transformation primitives 
	- Alphanumeric character drawing and scrolling capability 
	
3. Converters
  * [MCP2003](http://ww1.microchip.com/downloads/en/DeviceDoc/22230a.pdf) and [ADS1015](http://www.ti.com/product/ads1015) Analogue Digital Converters (ADC).
  
4. Drivers
	* [Ht16K33 16*8 LED Controller Driver](http://www.holtek.com/english/docum/consumer/16K33.htm) driving Adafruit Mini 8x8 LED Matrix.  See Adafruit project library.

3. Command and Control
 * Control relays etc via the communications layer

4. Communications
 * Pluggable – currently implemented on MQTT ([Mosquitto](http://mosquitto.org) MQTT Server running on Azure)

5. Supported and Tested
 * Windows 10 for IoT on Raspberry Pi 2 
 * Supports Visual Studio 2015


## IoT Dashboard
The IoT Dashboard allows you to visualise the data streamed to Azure. 

![IoT Dashboard](https://github.com/MakerDen/IoT-Maker-Den-NETMF/blob/master/MakerDen/Lab%20Code/IoTDashboard.JPG)

You can install the IoT Dashboard from [here](http://iotmakerdendashboard.azurewebsites.net/install/publish.htm).  Note, you will need to allow to run from unknown publisher.


##Creating your first applications


### First App

The only requirement is an Internet connected Raspberry Pi 2 running Windows 10 for IoT

**This example will publish data to Azure and the IoT Dashboard**

1. From Visual Studio 2015 open the MakerDen.cs file in the MakerDen Project
2. Modify the DevId in the StartNetwork Services method
3. Ensure your Raspberry Pi is internet connected
4. Ensure Visual Studio set to deploy to your "Remote Machine"
5. Click Start to compile, deploy and run your application on the Raspberry Pi
6. Open the IoT Dashboard and your data will appear as a guage

**MakerDen.cs**
    
    using Glovebox.Adafruit.Mini8x8Matrix;
    using Glovebox.IO.Components;
    using Glovebox.IO.Components.Actuators;
    using Glovebox.IO.Components.Sensors;
    using Glovebox.IoT;
    using System.Threading;
    
    namespace MakerDen
    {
        public class MakerDen : IoTServices
        {        
            public MakerDen() : base(false) { } // if Explorer Hat installed set : base(true)
    
            public void Main()
            {
                // Replace "myPi2" with a unique 3 to 5 character identifier. Use your initials or something similar.  
                // This identifier will be visible on the IoT Dashboard
                StartNetworkServices("myPi2", "YourNetworkId");
    
                using (SensorMemory memory = new SensorMemory(5000, "mem01"))
                {
                    memory.OnAfterMeasurement += OnAfterMeasurement;
                    Util.Delay(Timeout.Infinite);
                }
            }  //  End of Main()
        }
    }

###Blinky

Blinky is the "[Hello, World!](http://en.wikipedia.org/wiki/%22Hello,_World!%22_program)" of the Internet of Things World

This example assumes you have a Explorer Pro HAT

**MakerDen.cs**

    using Glovebox.Adafruit.Mini8x8Matrix;
    using Glovebox.IO.Components;
    using Glovebox.IO.Components.Actuators;
    using Glovebox.IO.Components.Sensors;
    using Glovebox.IoT;
    using System.Threading;
    
    namespace MakerDen
    {
        public class MakerDen : IoTServices
        {
            public InitMakerDen() : base(true) { } // if Explorer Hat installed set : base(true)
    
            public void Main()
            {
                using (Led red = ExplorerLedRed)
                {
                    while (true)
                    {
                        red.On();
                        Util.Delay(1000);
                        red.Off();
                        Util.Delay(1000);
                    }
                }
            } // End of Main()
        }
    }
        
        
Or wire up your own LED.  Follow this [wiring diagram](http://ms-iot.github.io/content/en-US/win10/samples/Blinky.htm)

**MakerDen.cs**

    using Glovebox.Adafruit.Mini8x8Matrix;
    using Glovebox.IO.Components;
    using Glovebox.IO.Components.Actuators;
    using Glovebox.IO.Components.Sensors;
    using Glovebox.IoT;
    using System.Threading;
    
    namespace MakerDen
    {
        public class MakerDen : IoTServices
        {
            public InitMakerDen() : base(false) { } // if Explorer Hat installed set : base(true)
                      
            public void Main()
            {
                using (Led red = new Led(18, "led"))
                {
                    while (true)
                    {
                        red.On();
                        Util.Delay(1000);
                        red.Off();
                        Util.Delay(1000);
                    }
                }
            } // End of Main()
        }
    }



###Light sensor activated light

**MakerDen.cs**

    using Glovebox.Adafruit.Mini8x8Matrix;
    using Glovebox.IO.Components;
    using Glovebox.IO.Components.Actuators;
    using Glovebox.IO.Components.Sensors;
    using Glovebox.IoT;
    using System.Threading;
    
    namespace MakerDen
    {
        public class MakerDen : IoTServices
        {                
            public InitMakerDen() : base(true) { } // if Explorer Hat installed set : base(true)
    
            public void Main()
            {
                using (SensorLight light = new SensorLight(adc, Timeout.Infinite, "light01"))
                using (Led redLed = ExplorerLedRed)
                {
                    while (true)
                    {
                        if (light.Current < 70)
                        {
                            redLed.On();
                        }
                        else
                        {
                            redLed.Off();
                        }
                        Util.Delay(500);
                    }
                }
            } // End of Main()
        }
    }

###Ultimate

All singing dancing with Explorer Hat, Light and Temperature Sensors plus Adafruit Mini 8x8 LED Matrix that publishes data to Azure :)

**MakerDen.cs**

    using Glovebox.Adafruit.Mini8x8Matrix;
    using Glovebox.IO.Components;
    using Glovebox.IO.Components.Actuators;
    using Glovebox.IO.Components.Sensors;
    using Glovebox.IoT;
    using System.Threading;
    
    namespace MakerDen
    {
        public class MakerDen : IoTServices
        {        
            public InitMakerDen() : base(true) { } // if Explorer Hat installed set : base(true)
    
            public void Main()
            {
                // Replace "myPi2" with a unique 3 to 5 character identifier. Use your initials or something similar.  
                // This identifier will be visible on the IoT Dashboard
                StartNetworkServices("myPi2", "YourNetworkId");
    
                using (Relay relay = new Relay(ExplorerPins.Output.One, "relay01"))
                using (SensorTemp temp = new SensorTemp(adc, 4000, "temp01"))
                using (SensorLight light = new SensorLight(adc, 1000, "light01"))
                using (SensorMemory memory = new SensorMemory(5000, "mem01"))
                using (AdaFruitMatrixRun matrixRun = new AdaFruitMatrixRun("matrix01"))
                {
                    //Enable data publishing
                    temp.OnBeforeMeasurement += OnBeforeMeasure;
                    temp.OnAfterMeasurement += OnAfterMeasurement;
    
                    light.OnBeforeMeasurement += OnBeforeMeasure;
                    light.OnAfterMeasurement += OnAfterMeasurement;
    
                    memory.OnBeforeMeasurement += OnBeforeMeasure;
                    memory.OnAfterMeasurement += OnAfterMeasurement;
    
                    Util.Delay(Timeout.Infinite);
                }
            } //end of Main()
        }
    }

See Lab code snippets for more examples


##Explorer Hat Layouts
###Layout One

####Required components
2. [Explorer HAT Pro](http://shop.pimoroni.com/products/explorer-hat)
3. Light Dependent Resistor
4. [Analogue Temperature Sensor](http://au.rs-online.com/web/p/temperature-humidity-sensors/0403838/)

|Layout         |Image         |
|---------------|--------------|
|Wiring | ![Basic wiring](https://github.com/MakerDen/IoT-Maker-Den-Windows-for-IoT/blob/master/MakerDen/Lab%20Code/Images/ExplorerHat%20Basic%20Wiring.jpg) |
|Components | ![Explorer Hat Pro Layout (Basic)](https://github.com/MakerDen/IoT-Maker-Den-Windows-for-IoT/blob/master/MakerDen/Lab%20Code/Images/ExplorerHat%20Basic.jpg)|


###Layout Two
####Required components
1. [Explorer HAT Pro](http://shop.pimoroni.com/products/explorer-hat)
2. Light Dependent Resistor
3. [Analogue Temperature Sensor](http://au.rs-online.com/web/p/temperature-humidity-sensors/0403838/)
4. [Adafruit Mini 8x8 LED Matrix](http://tronixlabs.com/display/led/matrix/adafruit-small-1-2-8x8-led-matrix-w-i2c-backpack-red/)

|Layout         |Image         |
|---------------|--------------|
|Wiring | ![Just wiring](https://github.com/MakerDen/IoT-Maker-Den-Windows-for-IoT/blob/master/MakerDen/Lab%20Code/Images/ExplorerHat%20with%20I2C%20Adafruit%20Matrix%20Wiring.jpg) |
|Components | ![Explorer Hat Pro Layout (with Adafruit mini 8x8 matrix)](https://github.com/MakerDen/IoT-Maker-Den-Windows-for-IoT/blob/master/MakerDen/Lab%20Code/Images/ExplorerHat%20with%20I2C%20Adafruit%20Matrix.jpg)|

###Layout Three
####Required components
1. [Explorer HAT Pro](http://shop.pimoroni.com/products/explorer-hat)
2. Light Dependent Resistor
3. [Analogue Temperature Sensor](http://au.rs-online.com/web/p/temperature-humidity-sensors/0403838/)
4. [Adafruit Mini 8x8 LED Matrix](http://tronixlabs.com/display/led/matrix/adafruit-small-1-2-8x8-led-matrix-w-i2c-backpack-red/)
5. [Breakout Board for Electret Microphone](http://littlebirdelectronics.com.au/products/breakout-board-for-electret-microphone)

|Layout         |Image         |
|---------------|--------------|
|Wiring | ![Explorer Hat Pro Layout (with additional analog for sound sensor plus Adafruit mini 8x8 matrix)](https://github.com/MakerDen/IoT-Maker-Den-Windows-for-IoT/blob/master/MakerDen/Lab%20Code/Images/ExplorerHat%20with%20additional%20analog%20and%20Adafruit%20matrix%20wiring.jpg) | 
|Components | ![Explorer Hat Pro Layout (with additional analog for sound sensor plus Adafruit mini 8x8 matrix)](https://github.com/MakerDen/IoT-Maker-Den-Windows-for-IoT/blob/master/MakerDen/Lab%20Code/Images/ExplorerHat%20with%20additional%20analog%20and%20Adafruit%20matrix.jpg)|

