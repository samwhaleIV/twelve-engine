using System;
using System.Collections.Generic;
using TwelveEngine.Serial;

namespace Porthole.PuzzleGame {
    public abstract class Component:ISerializable {

        public bool StateLock { get; set; } = false;
        public Component Input { get; set; } = null;
        public readonly List<Component> Outputs = new List<Component>();

        public Action StateChanged { get; set; }
        public SignalState SignalState = SignalState.Neutral;

        protected virtual void UpdateSignal() {
            if(Input == null) {
                return;
            }
            SignalState = Input.SignalState;
        }

        public void SendSignal() {
            var oldState = SignalState;
            UpdateSignal();
            StateChanged?.Invoke();
            foreach(var output in Outputs) {
                output.SendSignal();
            }
        }

        public Component Link(Component component) {
            Outputs.Add(component);
            component.Input = this;
            return component;
        }

        public virtual void Export(SerialFrame frame) {
            frame.Set((int)SignalState);
        }

        public virtual void Import(SerialFrame frame) {
            SignalState = (SignalState)frame.GetInt();
        }
    }
}
