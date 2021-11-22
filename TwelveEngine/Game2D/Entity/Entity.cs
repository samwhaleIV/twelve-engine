namespace TwelveEngine.Game2D {
    public class Entity:ISerializable {
        public string FactoryID = null;
        public string Name = string.Empty;

        public GameManager Game;
        public Grid2D Grid;

        public long ID = 0;
        public EntityManager Owner = null;

        public float X = 0;
        public float Y = 0;
        public float Width = 1;
        public float Height = 1;

        public virtual void Load() { }
        public virtual void Unload() { }

        public virtual void Export(SerialFrame frame) {
            frame.Set("X",X);
            frame.Set("Y",Y);
            frame.Set("Width",Width);
            frame.Set("Height",Height);
        }
        public virtual void Import(SerialFrame frame) {
            X = frame.GetFloat("X");
            Y = frame.GetFloat("Y");
            Width = frame.GetFloat("Width");
            Height = frame.GetFloat("Height");
        }
    }
}
