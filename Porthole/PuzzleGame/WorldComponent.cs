using TwelveEngine;
using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Collision.Tile;

namespace Porthole.PuzzleGame {
    public abstract class WorldComponent:Component {

        private readonly TileGrid grid;
        protected TileGrid Grid => grid;

        protected int[,] ObjectLayer => grid.GetLayer(Constants.ObjectLayerIndex);
        protected int[,] CollisionLayer => grid.GetLayer(Constants.CollisionLayerIndex);
        
        public WorldComponent(TileGrid grid) {
            this.grid = grid;
            StateChanged = OnChange;
        }

        protected abstract void OnChange();
    }
}
