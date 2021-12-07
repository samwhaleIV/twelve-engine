using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TwelveEngine {
    public enum KeyBind {

        /* Don't change the order of the key binds enum.
         * A strict order allows for backwards compatibility */

        Up, Down, Left, Right, Accept, Exit
    }
}
namespace TwelveEngine.Input {
    public sealed class KeyBinds:ISerializable {

        private readonly Dictionary<KeyBind,Keys> binds = new Dictionary<KeyBind,Keys>() {
            {KeyBind.Up,Keys.W},
            {KeyBind.Down,Keys.S},
            {KeyBind.Left,Keys.A},
            {KeyBind.Right,Keys.D},
            {KeyBind.Accept,Keys.Enter},
            {KeyBind.Exit,Keys.Escape}
        };

        public Keys Up {
            get => binds[KeyBind.Up];
            set => binds[KeyBind.Up] = value;
        }
        public Keys Down {
            get => binds[KeyBind.Down];
            set => binds[KeyBind.Down] = value;
        }
        public Keys Left {
            get => binds[KeyBind.Left];
            set => binds[KeyBind.Left] = value;
        }
        public Keys Right {
            get => binds[KeyBind.Right];
            set => binds[KeyBind.Right] = value;
        }
        public Keys Accept {
            get => binds[KeyBind.Accept];
            set => binds[KeyBind.Accept] = value;
        }
        public Keys Exit {
            get => binds[KeyBind.Exit];
            set => binds[KeyBind.Exit] = value;
        }

        public Keys Get(KeyBind type) => binds[type];
        public void Set(KeyBind bind,Keys key) => binds[bind] = key;

        public void Export(SerialFrame frame) {
            frame.Set(binds.Count);
            foreach(var bind in binds) {
                frame.Set(bind.Key);
                frame.Set(bind.Value);
            }
        }

        public void Import(SerialFrame frame) {
            int count = frame.GetInt();
            for(var i = 0;i<count;i++) {
                var bind = frame.GetBind();
                var key = frame.GetKey();
                binds[bind] = key;
            }
        }
    }
}
