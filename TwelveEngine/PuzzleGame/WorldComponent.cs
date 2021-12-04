using TwelveEngine.Game2D;

namespace TwelveEngine.PuzzleGame {
    public abstract class WorldComponent:Component {

        protected readonly Grid2D grid;

        protected int[,] ObjectLayer => grid.GetLayer(Constants.ObjectLayerIndex);
        protected int[,] CollisionLayer => grid.GetLayer(Constants.CollisionLayerIndex);
        
        public WorldComponent(Grid2D grid) {
            this.grid = grid;
            StateChanged = OnChange;
        }

        protected abstract void OnChange();
    }
}
