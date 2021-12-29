using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {
    public sealed partial class KeyBindSet:ISerializable {

        private readonly Dictionary<Impulse,Keys> binds = GetKeyBinds();

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

        public Keys this[Impulse type] {
            set => binds[type] = value;
            get => binds[type];
        }

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
