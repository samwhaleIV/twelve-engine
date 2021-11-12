using Microsoft.Xna.Framework;

namespace TwelveEngine {
    public interface ITileRenderer<T> where T:struct {
        void Load(GameManager game,Grid2D<T> grid2D);
        void Unload();
        void RenderTile(T value,Rectangle destination);
    }
}
