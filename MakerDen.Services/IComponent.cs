using MakerDen.Command;

namespace MakerDen {

    public enum Class {
        Actuator, Sensor
    }

    public interface IComponent {
        Class Class { get; }
        string Name { get; }
        string Type { get; }

        void Action(IotAction action);
    }
}