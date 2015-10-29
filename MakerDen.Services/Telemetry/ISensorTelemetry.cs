using MakerDen.Sensor;

namespace MakerDen.Telemetry {
    public interface ISensorTelemetry {

        string Channel { get; }

        byte[] ToJson();

        string ToString();

        ISensor Sensor { get; }
    }
}