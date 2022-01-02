using System.Collections.Generic;
using TwelveEngine.Game2D;

namespace TwelveEngine.Porthole.PuzzleGame {
    public sealed class PuzzleManager:ISerializable {

        private readonly Component[] components;

        public PuzzleManager(Grid2D grid,Component[] components) {
            this.components = components;
            installInteractionLayer(grid);
            installStateHandlers(grid);
        }

        private void installInteractionLayer(Grid2D grid) {
            var interactionLayer = new InteractionLayer();
            grid.Interaction = interactionLayer;

            foreach(var component in components) {
                if(!(component is IInteract)) {
                    continue;
                }
                interactionLayer.AddTarget((IInteract)component);
            }
        }

        private void installStateHandlers(Grid2D grid) {
            grid.OnExport += frame => frame.Set(this);
            grid.OnImport += frame => frame.Get(this);
        }

        public void Import(SerialFrame frame) {
            var updateComponents = new List<Component>();
            for(var i = 0;i<components.Length;i++) {
                var component = components[i];
                if(component.StateLock) {
                    component.StateChanged?.Invoke();
                    continue;
                }
                var oldState = component.SignalState;
                frame.Get(component);
                var newState = component.SignalState;

                if(newState != oldState) {
                    updateComponents.Add(component);
                }
            }
            foreach(var component in updateComponents) {
                component.SendSignal();
            }
        }
        public void Export(SerialFrame frame) {
            for(var i = 0;i<components.Length;i++) {
                var component = components[i];
                if(component.StateLock) {
                    continue;
                }
                frame.Set(component);
            }
        }
    }
}
