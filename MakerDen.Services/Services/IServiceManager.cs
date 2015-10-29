namespace MakerDen.Services {
    public interface IServiceManager {

        //void Initialise();
        uint Publish(string topic, byte[] data);
    }
}