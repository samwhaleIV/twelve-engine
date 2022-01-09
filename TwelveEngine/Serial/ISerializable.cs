using TwelveEngine.Serial;

namespace TwelveEngine {
    public interface ISerializable {
        void Export(SerialFrame frame);
        void Import(SerialFrame frame);
    }
}
