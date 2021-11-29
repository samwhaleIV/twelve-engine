namespace TwelveEngine.PuzzleGame {
    public abstract class WorldInterface:Component, ISerializable {
        private readonly int[,] objectLayer;
        private readonly int[,] collisionLayer;

        public bool Serialize { get; set; } = true;
        
        public int[,] ObjectLayer => objectLayer;
        public int[,] CollisionLayer => collisionLayer;

        public WorldInterface(int[,] objectLayer,int[,] collisionLayer) {
            this.objectLayer = objectLayer;
            this.collisionLayer = collisionLayer;

            StateChanged = OnChange;
        }

        public abstract void OnChange(SignalState state);
        public virtual void Export(SerialFrame frame) {
            if(!Serialize) return;
            frame.Set("State",(int)SignalState);
        }
        public virtual void Import(SerialFrame frame) {
            if(!Serialize) return;
            SignalState = (SignalState)frame.GetInt("State");
            SendSignal();
        }
    }
}
