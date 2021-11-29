using TwelveEngine.Game2D;

namespace TwelveEngine.PuzzleGame {
    public sealed class PuzzleBoard:ISerializable {

        private readonly ComponentFactory factory;
        private readonly WorldInterface[] gameComponents;
        private readonly int componentCount;

        public PuzzleBoard(Grid2D grid,Level level) {
            factory = new ComponentFactory(grid.GetLayer(1),grid.GetLayer(2));
            gameComponents = level.ComponentGenerator(factory);
            componentCount = gameComponents.Length;

            var interactionLayer = new InteractionLayer();

            foreach(var component in gameComponents) {
                if(!(component is IInteract)) {
                    continue;
                }
                interactionLayer.AddTarget((IInteract)component);
            }

            grid.InteractionLayer = interactionLayer;
        }

        public void Export(SerialFrame frame) {
            for(var i = 0;i<componentCount;i++) {
                frame.Set(i.ToString(),gameComponents[i]);
            }
        }

        public void Import(SerialFrame frame) {
            for(var i = 0;i<componentCount;i++) {
                frame.Get(i.ToString(),gameComponents[i]);
            }
        }
    }
}
