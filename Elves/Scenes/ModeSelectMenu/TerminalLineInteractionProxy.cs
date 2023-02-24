using System;
using TwelveEngine.UI;
using TwelveEngine;

namespace Elves.Scenes.ModeSelectMenu {
    public sealed class TerminalLineInteractionProxy:InteractionElement<TerminalLineInteractionProxy> {

        public Func<int,FloatRectangle> GetArea { get; init; }
        public Func<bool> GetInteractionPaused { get; init; }

        protected override bool GetInputPaused() {
            return GetInteractionPaused?.Invoke() ?? false;
        }
        protected override void SetInputPaused(bool value) {
            throw new NotImplementedException();
        }

        public int CommandIndex { get; init; }

        public override FloatRectangle GetScreenArea() => GetArea(CommandIndex);
        public TerminalLineInteractionProxy() => Endpoint = new Endpoint(() => OnActivated?.Invoke());
        public event Action OnActivated;
    }
}
