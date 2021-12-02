using System.Collections.Generic;
using TwelveEngine.Game2D;

namespace TwelveEngine.PuzzleGame {
    public sealed class PuzzleManager:ISerializable {

        private readonly Component[] components;

        public PuzzleManager(Grid2D grid,Component[] components) {
            this.components = components;
            installInteractionLayer(grid);
            installStateHandlers(grid);
        }

        private void installInteractionLayer(Grid2D grid) {
            var interactionLayer = new InteractionLayer();
            grid.InteractionLayer = interactionLayer;

            foreach(var component in components) {
                if(!(component is IInteract)) {
                    continue;
                }
                interactionLayer.AddTarget((IInteract)component);
            }
        }

        private void installStateHandlers(Grid2D grid) {
            grid.OnExport += frame => frame.Set("Puzzle",this);
            grid.OnImport += frame => frame.Get("Puzzle",this);
        }

        public void Import(SerialFrame frame) {
            componentStateIterator(frame,true);
        }
        public void Export(SerialFrame frame) {
            componentStateIterator(frame,false);
        }

        private static string getComponentIndex(int index) => $"p{index}";

        private void componentStateIterator(SerialFrame frame,bool import) {
            var updateComponents = import ? new List<Component>() : null;
            for(var i = 0;i<components.Length;i++) {
                var component = components[i];
                if(import && component.ComplexState) {
                    updateComponents.Add(component);
                }
                if(component.StateLock) {
                    continue;
                }
                var key = getComponentIndex(i);
                if(import) {
                    frame.Get(key,component);
                } else {
                    frame.Set(key,component);
                }
            }
            if(!import) {
                return;
            }
            foreach(var component in updateComponents) {
                component.SendSignal();
            }
        }
    }
}
