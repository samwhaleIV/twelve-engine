using TwelveEngine.Shell.States;
using Microsoft.Xna.Framework;
using TwelveEngine.Game3D;
using TwelveEngine.Game3D.Entity.Types;
using Elves.UI;
using TwelveEngine;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Elves {
    public sealed class MainMenu:OrthoBackgroundState {

        const string MENU_SPRITE_ATLAS = "Menu/falling-elf";

        private static readonly Color BACKGROUND_TOP_COLOR = Color.FromNonPremultiplied(0,225,89,255);
        private static readonly Color BACKGROUND_BOTTOM_COLOR = Color.Black;

        private const float FOREGROUND_WATER_OPACITY = 0.592f;
        private static Color FOREGROUND_WATER_COLOR => Color.FromNonPremultiplied(109,228,255,255);

        public MainMenu():base(UITextures.Nothing) {
            SetBackgroundColors(BACKGROUND_TOP_COLOR,BACKGROUND_TOP_COLOR,BACKGROUND_BOTTOM_COLOR,BACKGROUND_BOTTOM_COLOR);
            OnLoad += MainMenu_OnLoad;
        }

        private Screenspace3DSprite fallingElf;
        private TextureEntity waterOverlay;

        private void MainMenu_OnLoad() {
            var texture = Game.Content.Load<Texture2D>(MENU_SPRITE_ATLAS);
            fallingElf = new Screenspace3DSprite(texture) {
                TextureSource = new Rectangle(0,0,41,29)
            };
            waterOverlay = new TextureEntity(UITextures.Nothing) {
                PixelSmoothing = true,
                Color = FOREGROUND_WATER_COLOR,
                Alpha = FOREGROUND_WATER_OPACITY,
                Billboard = true,
                Scale = new Vector3(1f)
            };
            waterOverlay.SetUVArea(0,32,16,16);
            fallingElf.OnUpdate += FallingElf_OnUpdate;

            Entities.Add(fallingElf);
            Entities.Add(waterOverlay);
        }

        private void FallingElf_OnUpdate(GameTime gameTime) {
            Rectangle bounds = Game.Viewport.Bounds;
            float scale = Game.Viewport.Height / 60;
            Vector2 center = new Vector2(bounds.Width * 0.5f,bounds.Height * (2f/3f));
            Vector2 size = new Vector2(41,29) * scale;
            var t = (float)(gameTime.TotalGameTime / TimeSpan.FromSeconds(8) % 1);
            var t2 = (float)(gameTime.TotalGameTime / TimeSpan.FromSeconds(16) % 1);
            var offset = MathF.Sin(MathF.PI * 2 * t);
            var offset2 = MathF.Cos(MathF.PI * 2 * t2);
            center.X = center.X + offset * 10f;
            center.Y = center.Y + offset2 * 5f;
            fallingElf.Area = new VectorRectangle(center-size*0.5f,size);
        }
    }
}
