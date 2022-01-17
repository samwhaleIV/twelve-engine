﻿using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input.Glyphs {
    internal sealed class KeyboardMap:GlyphMap<Keys> {
        public KeyboardMap() {
            GlyphSize = 16;
            BlockColumns = 8;
        }
        protected override Keys[] GetList() => GetKeys();
        public static Keys[] GetKeys() => new Keys[] {
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
    }
}
