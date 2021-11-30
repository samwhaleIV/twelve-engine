using TwelveEngine.Game2D;

namespace TwelveEngine.PuzzleGame {
    public abstract class WorldComponent:Component, ISerializable {

        public bool Serialize { get; set; } = false;

        private readonly Grid2D grid;

        public int[,] ObjectLayer => grid.GetLayer(Constants.ObjectLayerIndex);
        public int[,] CollisionLayer => grid.GetLayer(Constants.CollisionLayerIndex);
        
        public WorldComponent(Grid2D grid) {
            this.grid = grid;
            StateChanged = OnChange;
        }

        public abstract void OnChange(SignalState state);
        public virtual void Export(SerialFrame frame) {
            if(!Serialize) {
                return;
            }
            frame.Set("State",(int)SignalState);
        }
        public virtual void Import(SerialFrame frame) {
            if(!Serialize) {
                return;
            }
            SignalState = (SignalState)frame.GetInt("State");
            SendSignal();
        }
    }
}
