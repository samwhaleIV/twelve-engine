namespace TwelveEngine.PuzzleGame.Components {
    public class LaserSplitter:WorldInterface {

        private readonly LaserDirection direction;
        public LaserDirection Direction => direction;

        public LaserSplitter(int[,] objectLayer,int[,] collisionLayer,LaserDirection direction) : base(objectLayer,collisionLayer) {
            this.direction = direction;
        }

        public override void OnChange(SignalState state) {
            //todo
        }

    }
}
