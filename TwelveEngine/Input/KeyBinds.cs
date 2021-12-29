using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {
    public sealed partial class KeyBindSet {
        private static Dictionary<Impulse,Keys> GetKeyBinds() => new Dictionary<Impulse,Keys>() {
            { Impulse.Up, Keys.W },
            { Impulse.Down, Keys.S },
            { Impulse.Left, Keys.A },
            { Impulse.Right, Keys.D },
            { Impulse.Accept, Keys.Enter} ,
            { Impulse.Exit, Keys.Escape }
        };
    }
}
