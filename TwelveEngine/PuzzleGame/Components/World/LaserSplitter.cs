using TwelveEngine.Game2D;

namespace TwelveEngine.PuzzleGame.Components {
    public class LaserSplitter:WorldComponent {

        private readonly LaserDirection direction;
        public LaserDirection Direction => direction;

        public LaserSplitter(Grid2D grid,LaserDirection direction) : base(grid) {
            this.direction = direction;
        }

        protected override void OnChange() {
            //todo
        }

    }
}
