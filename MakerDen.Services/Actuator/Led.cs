using Glovebox.IoT.Devices.Actuators;
using MakerDen.Command;
using System;

namespace MakerDen.Actuator
{
    public class Led : OutputPin, IDisposable, IComponent
    {

        public string Name { get; set; } = "led";

        public string Type { get; set; } = "Led";

        public Class Class { get; set; } = Class.Actuator;

        public Led(int pinNumber) : base(pinNumber)
        {
            IotActionManager.AddComponent(this);
        }


        public void Action(IotAction action)
        {
            if (action.cmd == null) { return; }
            switch (action.cmd)
            {
                case "on":
                    this.On();
                    break;
                case "off":
                    this.Off();
                    break;
            }
        }

        public new void Dispose()
        {
            IotActionManager.RemoveComponent(this);
            base.Dispose();
        }
    }
}
