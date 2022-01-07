using TwelveEngine;
using TwelveEngine.Game2D;

namespace Porthole.PuzzleGame {
    public abstract class WorldComponent:Component {

        private readonly Grid2D grid;
        protected Grid2D Grid => grid;

        protected int[,] ObjectLayer => grid.GetLayer(Constants.ObjectLayerIndex);
        protected int[,] CollisionLayer => grid.GetLayer(Constants.CollisionLayerIndex);
        
        public WorldComponent(Grid2D grid) {
            this.grid = grid;
            StateChanged = OnChange;
        }

        protected abstract void OnChange();
    }
}
