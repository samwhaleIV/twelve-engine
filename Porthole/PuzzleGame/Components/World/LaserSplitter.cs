namespace Porthole.PuzzleGame.Components {
    public class LaserSplitter:WorldComponent {

        private readonly LaserDirection direction;
        public LaserDirection Direction => direction;

        public LaserSplitter(PuzzleGrid grid,LaserDirection direction) : base(grid) {
            this.direction = direction;
        }

        protected override void OnChange() {
            //todo
        }

    }
}
