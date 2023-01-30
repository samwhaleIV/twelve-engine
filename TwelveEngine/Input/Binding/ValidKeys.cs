using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TwelveEngine.Input.Binding {
    internal static class ValidKeys {

        private static readonly Keys[] ValidKeysList = new Keys[] {
            Keys.OemTilde,
            Keys.D1,
            Keys.D2,
            Keys.D3,
            Keys.D4,
            Keys.D5,
            Keys.D6,
            Keys.D7,
            Keys.Tab,
            Keys.Q,
            Keys.W,
            Keys.E,
            Keys.R,
            Keys.T,
            Keys.Y,
            Keys.U,
            Keys.Space,
            Keys.A,
            Keys.S,
            Keys.D,
            Keys.F,
            Keys.G,
            Keys.H,
            Keys.J,
            Keys.LeftShift,
            Keys.Z,
            Keys.X,
            Keys.C,
            Keys.V,
            Keys.B,
            Keys.N,
            Keys.M,
            Keys.Up,
            Keys.Down,
            Keys.Left,
            Keys.Right,
            Keys.Insert,
            Keys.Home,
            Keys.PageUp,
            Keys.PageDown,
            Keys.D8,
            Keys.D9,
            Keys.D0,
            Keys.OemMinus,
            Keys.OemPlus,
            Keys.Back,
            Keys.OemComma,
            Keys.OemPeriod,
            Keys.I,
            Keys.O,
            Keys.P,
            Keys.OemOpenBrackets,
            Keys.OemCloseBrackets,
            Keys.OemBackslash,
            Keys.OemQuestion,
            Keys.LeftControl,
            Keys.K,
            Keys.L,
            Keys.OemSemicolon,
            Keys.OemQuotes,
            Keys.Enter,
            Keys.Delete,
            Keys.End,
            Keys.Escape
        };

        private static readonly HashSet<Keys> validKeys = GetValidKeysSet();

        public static bool Contains(Keys key) => validKeys.Contains(key);

        public static bool Contains(ref MultiBindKey key) {
            bool key1Valid, key2Valid;
            key1Valid = Contains(key.Bind);
            key2Valid = Contains(key.AltBind);
            if(key1Valid && key2Valid) {
                return true;
            }
            if(key1Valid) {
                key = new(key.Bind,Keys.None);
                return true;
            } else if(key2Valid) {
                key = new(key.AltBind,Keys.None);
                return true;
            } else {
                return false;
            }
        }

        private static HashSet<Keys> GetValidKeysSet() {
            HashSet<Keys> hashSet = new();
            Keys[] list = ValidKeysList;
            foreach(var key in list) {
                hashSet.Add(key);
            }
            return hashSet;
        }
    }
}
