﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Elves {
    public sealed class Textures {

        internal static ContentManager ContentManager { get; set; }
        private static Texture2D Load(string file) => ContentManager.Load<Texture2D>(file);

        public readonly Texture2D Panel = Load("panel");
        public readonly Texture2D Nothing = Load("nothing-white");
        public readonly Texture2D CursorDefault = Load("Cursor/default");
        public readonly Texture2D CursorAlt1 = Load("Cursor/alt-1");
        public readonly Texture2D CursorAlt2 = Load("Cursor/alt-2");
        public readonly Texture2D CursorNone = Load("Cursor/none");
        public readonly Texture2D Drowning = Load("drowning");
        public readonly Texture2D Lock = Load("lock");
        public readonly Texture2D Missing = Load("missing");
        public readonly Texture2D Mountains = Load("mountains");
        public readonly Texture2D SaveSelect = Load("save-select");
        public readonly Texture2D GiftPattern = Load("gift-pattern");
        public readonly Texture2D CircleBrush = Load("circle");

        public readonly Texture2D Static = Load("static");
        public readonly Texture2D Warning = Load("warning");

        public readonly Texture2D Carousel = Load("carousel");

        public readonly Texture2D SettingsPhone = Load("settings-phone");

        public readonly Texture2D MiniGameTablet = Load("minigame-tablet");
        public readonly Texture2D CRTStencil = Load("crt-stencil");

        public readonly Texture2D ElvesLogo = Load("Logos/elves");
        public readonly Texture2D FMODLogo = Load("Logos/fmod");
        public readonly Texture2D MonoGameLogo = Load("Logos/monogame");
        public readonly Texture2D TwelveEngineLogo = Load("Logos/twelve-engine");
    }
}
