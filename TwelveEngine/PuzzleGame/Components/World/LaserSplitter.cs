namespace TwelveEngine.PuzzleGame.Components.World {
    public class LaserSplitter:WorldInterface {

        private readonly Direction direction;

        public LaserSplitter(int[,] objectLayer,int[,] collisionLayer,Direction direction) : base(objectLayer,collisionLayer) {
            this.direction = direction;
        }

        public override void OnChange(SignalState state) {
            //todo
        }

        public enum Direction {
            TopLeft, TopMiddle, TopRight,
            MiddleLeft, Middle, MiddleRight,
            BottomLeft, BottomMiddle, BottomRight
        }

    }
}
