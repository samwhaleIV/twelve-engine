using System;
using System.Collections.Generic;
using TwelveEngine.Shell;
using TwelveEngine.UI;

namespace Elves.Scenes.ModeSelectMenu {
    public sealed class ModeSelectMenuInteractionAgent:InteractionAgent<TerminalLineInteractionProxy> {

        private readonly IEnumerable<TerminalLineInteractionProxy> _elements;
        private readonly ModeSelectMenuScene _owner;

        public ModeSelectMenuInteractionAgent(ModeSelectMenuScene owner,IEnumerable<TerminalLineInteractionProxy> elements) {
            _owner = owner;
            _elements = elements;
        }

        protected override bool GetContextTransitioning() {
            return _owner.IsTransitioning || _owner.IsWritingText;
        }

        protected override TimeSpan GetCurrentTime() {
            return _owner.Now;
        }

        protected override IEnumerable<TerminalLineInteractionProxy> GetElements() {
            return _elements;
        }

        protected override bool GetLeftMouseButtonIsCaptured() {
            return _owner.MouseHandler.CapturingLeft;
        }

        protected override bool GetRightMouseButtonIsCaptured() {
            return _owner.MouseHandler.CapturingRight;
        }
    }
}
