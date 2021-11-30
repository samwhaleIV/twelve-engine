using TwelveEngine.Game2D;

namespace TwelveEngine.PuzzleGame {
    public sealed class PuzzleBoard:ISerializable {

        private readonly WorldComponent[] gameComponents;

        private void installInteractionLayer(Grid2D grid) {
            var interactionLayer = new InteractionLayer();
            grid.InteractionLayer = interactionLayer;

            foreach(var component in gameComponents) {
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

        private WorldComponent[] getWorldComponents(Grid2D grid,Level level) {
            var factory = new ComponentFactory(grid);
            level.GenerateComponents(factory);
            return factory.Export();
        }

        public PuzzleBoard(Grid2D grid,Level level) { 
            gameComponents = getWorldComponents(grid,level);
            installInteractionLayer(grid);
            installStateHandlers(grid);
        }

        private static string getComponentIndex(int index) => $"Item{index}";

        public void Export(SerialFrame frame) {
            for(var i = 0;i<gameComponents.Length;i++) {
                frame.Set(getComponentIndex(i),gameComponents[i]);
            }
        }

        public void Import(SerialFrame frame) {
            for(var i = 0;i<gameComponents.Length;i++) {
                frame.Get(getComponentIndex(i),gameComponents[i]);
            }
        }
    }
}
