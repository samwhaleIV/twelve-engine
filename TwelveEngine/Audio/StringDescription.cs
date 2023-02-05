using FMOD;

namespace TwelveEngine.Audio {
    public readonly struct StringDescription {
        public readonly string Path { get; init; }
        public readonly GUID GUID { get; init; }
    }
}
