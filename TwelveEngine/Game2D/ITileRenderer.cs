using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D {
    public interface ITileRenderer {
        void Load(GameManager game,Grid2D grid2D);
        void Unload();
        void RenderTile(int value,Rectangle destination);
        void RenderTileDepth(int value,Rectangle destination,float viewportHeight);
    }
}
