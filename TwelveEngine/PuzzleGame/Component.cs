using System;
using System.Collections.Generic;

namespace TwelveEngine.PuzzleGame {
    public abstract class Component {

        public Component Input { get; set; } = null;

        public readonly List<Component> Outputs = new List<Component>();

        public Action<SignalState> StateChanged { get; set; }
        public SignalState SignalState = SignalState.Neutral;

        public virtual void UpdateSignal() {
            if(Input == null) {
                return;
            }
            SignalState = Input.SignalState;
        }

        public void SendSignal() {
            UpdateSignal();
            StateChanged?.Invoke(SignalState);
            foreach(var output in Outputs) {
                output.SendSignal();
            }
        }

        public Component Link(Component component) {
            Outputs.Add(component);
            component.Input = this;
            return component;
        }
    }
}
