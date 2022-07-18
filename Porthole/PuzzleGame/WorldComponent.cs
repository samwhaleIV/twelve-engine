using TwelveEngine;

namespace Porthole.PuzzleGame {
    public abstract class WorldComponent:Component {

        private readonly PuzzleGrid grid;
        protected PuzzleGrid Grid => grid;

        protected int[,] ObjectLayer => grid.GetLayer(Constants.ObjectLayerIndex);
        protected int[,] CollisionLayer => grid.GetLayer(Constants.CollisionLayerIndex);
        
        public WorldComponent(PuzzleGrid grid) {
            this.grid = grid;
            StateChanged = OnChange;
        }

        protected abstract void OnChange();
    }
}
