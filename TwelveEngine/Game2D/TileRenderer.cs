using System;

namespace TwelveEngine.Game2D {
    public abstract class TileRenderer {

        private GameManager game;
        private Grid2D grid;

        protected GameManager Game => game;
        protected Grid2D Grid => grid;

        protected event Action OnLoad;
        protected event Action OnUnload;

        internal void Load(GameManager game,Grid2D grid) {
            this.game = game;
            this.grid = grid;
            OnLoad?.Invoke();
        }
        internal void Unload() => OnUnload?.Invoke();

        public abstract void RenderTiles(ScreenSpace screenSpace,int[,] data);
    }
}
