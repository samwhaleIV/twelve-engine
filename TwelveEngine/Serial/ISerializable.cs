namespace TwelveEngine.Serial {
    public interface ISerializable {
        void Export(SerialFrame frame);
        void Import(SerialFrame frame);
    }
}
