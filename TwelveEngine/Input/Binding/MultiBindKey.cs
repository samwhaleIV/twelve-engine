using Microsoft.Xna.Framework.Input;
using System.IO;

namespace TwelveEngine.Input.Binding {
    public readonly struct MultiBindKey {

        public readonly Keys Bind, AltBind;

        public bool IsEmpty => Bind == Keys.None && AltBind == Keys.None;

        public bool IsPressed(in KeyboardState keyboardState) {
            return keyboardState.IsKeyDown(Bind) || keyboardState.IsKeyDown(AltBind);
        }

        public MultiBindKey(Keys key1) {
            Bind = key1;
            AltBind = Keys.None;
        }

        public MultiBindKey(Keys key1,Keys key2) {
            Bind = key1;
            AltBind = key2;
        }

        public static MultiBindKey Read(BinaryReader reader) {
            Keys key1 = (Keys)reader.ReadInt32();
            Keys key2 = (Keys)reader.ReadInt32();
            return new(key1,key2);
        }

        public void Write(BinaryWriter writer) {
            writer.Write((int)Bind);
            writer.Write((int)AltBind);
        }

        public static readonly MultiBindKey None = new(Keys.None,Keys.None);
        public static implicit operator MultiBindKey(Keys key) => new(key,Keys.None);
        public static implicit operator MultiBindKey((Keys a,Keys b) keys) => new(keys.a,keys.b);
    }
}
