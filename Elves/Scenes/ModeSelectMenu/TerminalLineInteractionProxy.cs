using System;
using TwelveEngine.UI;
using TwelveEngine;

namespace Elves.Scenes.ModeSelectMenu {
    public sealed class TerminalLineInteractionProxy:InteractionElement<TerminalLineInteractionProxy>,IEndpoint<ModeSelection> {

        public Func<int,FloatRectangle> GetArea { get; init; }
        public Func<bool> GetInteractionPaused { get; init; }

        public TerminalLineInteractionProxy() {
            Endpoint = new Endpoint<ModeSelection>(this);
        }

        public event Action<ModeSelection> OnActivated;

        protected override bool GetInputPaused() {
            return GetInteractionPaused?.Invoke() ?? false;
        }
        protected override void SetInputPaused(bool value) {
            throw new NotImplementedException();
        }

        public int CommandIndex { get; init; }

        public override FloatRectangle GetScreenArea() => GetArea(CommandIndex);

        public ModeSelection Selection { get; init; }

        public ModeSelection GetEndPointValue() {
            return Selection;
        }

        public void FireActivationEvent(ModeSelection value) {
            OnActivated?.Invoke(value);
        }
    }
}
