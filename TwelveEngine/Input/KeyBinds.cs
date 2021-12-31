using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {
    public sealed partial class KeyBindSet {
        private static Dictionary<Impulse,Keys> GetKeyBinds() => new Dictionary<Impulse,Keys>() {
            { Impulse.Up, KeyBinds.Up },
            { Impulse.Down, KeyBinds.Down },
            { Impulse.Left, KeyBinds.Left },
            { Impulse.Right, KeyBinds.Right },
            { Impulse.Accept, KeyBinds.Accept } ,
            { Impulse.Cancel, KeyBinds.Cancel }
        };
    }
}
