namespace TwelveEngine.Game2D {
    public abstract class Entity:ISerializable {
        public string FactoryID = null;
        public string Name = string.Empty;

        public GameManager Game;

        public long ID = 0;
        public EntityManager Owner = null;

        public virtual void Load() {}
        public virtual void Unload() {}

        public abstract void Export(SerialFrame frame);
        public abstract void Import(SerialFrame frame);
    }
}
