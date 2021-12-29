using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TwelveEngine.Input {
    public sealed class KeyBinds:ISerializable {

        private readonly Dictionary<Impulse,Keys> binds = new Dictionary<Impulse,Keys>() {
            {Impulse.Up,Keys.W},
            {Impulse.Down,Keys.S},
            {Impulse.Left,Keys.A},
            {Impulse.Right,Keys.D},
            {Impulse.Accept,Keys.Enter},
            {Impulse.Exit,Keys.Escape}
        };

        public Keys Up {
            get => binds[Impulse.Up];
            set => binds[Impulse.Up] = value;
        }
        public Keys Down {
            get => binds[Impulse.Down];
            set => binds[Impulse.Down] = value;
        }
        public Keys Left {
            get => binds[Impulse.Left];
            set => binds[Impulse.Left] = value;
        }
        public Keys Right {
            get => binds[Impulse.Right];
            set => binds[Impulse.Right] = value;
        }
        public Keys Accept {
            get => binds[Impulse.Accept];
            set => binds[Impulse.Accept] = value;
        }
        public Keys Exit {
            get => binds[Impulse.Exit];
            set => binds[Impulse.Exit] = value;
        }

        public Keys Get(Impulse type) => binds[type];
        public void Set(Impulse bind,Keys key) => binds[bind] = key;

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
