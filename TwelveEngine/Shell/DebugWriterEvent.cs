using TwelveEngine.Shell.UI;

namespace TwelveEngine.Shell {
    public sealed class DebugWriterEvent:OrderedEvent<Action<DebugWriter>> {
        public override void OnInvoke(Action<DebugWriter> action) => action.Invoke(Writer);
        public DebugWriter Writer { get; set; }
    }
}
