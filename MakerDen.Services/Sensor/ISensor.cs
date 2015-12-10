namespace MakerDen.Sensor
{
    public enum ValuesPerSample { One = 1, Two = 2, Three = 3, Four = 4, Five = 5 };

    public enum Trigger { Disabled, Triggered, None};

    public interface ISensor
    {
        int SampleRateMilliseconds { get; set; }

        string[] UnitofMeasure { get; }

        ValuesPerSample ValuesPerSample { get; }

        string GeoLocation { get; set; }

        void Measure();

        Trigger TriggerState { get; set; }

        int TriggerThreshold { get; set; }

        double[] Value { get; }
    }
}