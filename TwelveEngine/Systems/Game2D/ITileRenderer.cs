using Microsoft.Xna.Framework;

namespace TwelveEngine {
    public interface ITileRenderer {
        void Load(GameManager game,Grid2D grid2D);
        void Unload();
        void RenderTile(int value,Rectangle destination);
    }
}
