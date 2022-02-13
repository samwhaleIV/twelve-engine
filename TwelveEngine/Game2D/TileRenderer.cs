using System;
using TwelveEngine.Shell;

namespace TwelveEngine.Game2D {
    public abstract class TileRenderer {

        protected Grid2D Grid { get; private set; }
        protected GameManager Game { get; private set; }

        protected event Action OnLoad, OnUnload;

        internal void Load(Grid2D grid) {
            Grid = grid;
            Game = grid.Game;
            OnLoad?.Invoke();
        }
        internal void Unload() => OnUnload?.Invoke();

        public abstract void CacheArea(ScreenSpace screenSpace);
        public abstract void RenderTiles(int[,] data);
    }
}
