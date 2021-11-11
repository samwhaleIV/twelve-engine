using Microsoft.Xna.Framework;

namespace TwelveEngine {
    public abstract class GameState:ISerializable {

        internal abstract void Load(GameManager game);
        internal abstract void Unload();

        internal abstract void Draw(GameTime gameTime);
        internal abstract void Update(GameTime gameTime);

        public abstract void Export(SerialFrame frame);
        public abstract void Import(SerialFrame frame);
    }
}
