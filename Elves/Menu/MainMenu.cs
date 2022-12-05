using TwelveEngine.Shell.States;
using Microsoft.Xna.Framework;
using TwelveEngine.Game3D;
using TwelveEngine.Game3D.Entity.Types;
using Elves.UI;
using TwelveEngine;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Elves.Menu {
    public sealed class MainMenu:OrthoBackgroundState {

        const string MENU_SPRITE_ATLAS = "Menu/falling-elf";

        private static readonly Color BACKGROUND_TOP_COLOR = Color.FromNonPremultiplied(0,225,89,255);
        private static readonly Color BACKGROUND_BOTTOM_COLOR = Color.Black;

        private const float FOREGROUND_WATER_OPACITY = 0.592f;
        private static Color FOREGROUND_WATER_COLOR => Color.FromNonPremultiplied(109,228,255,255);

        public float GetPixelScale() => Game.Viewport.Height / 60;

        public MainMenu(bool debug3D = false):base(UITextures.Nothing,true,debug3D) {
            SetBackgroundColors(BACKGROUND_TOP_COLOR,BACKGROUND_TOP_COLOR,BACKGROUND_BOTTOM_COLOR,BACKGROUND_BOTTOM_COLOR);
            OnLoad += MainMenu_OnLoad;
        }

        private Screenspace3DSprite fallingElf;
        private TextureEntity waterOverlay;

        private readonly List<Screenspace3DSprite> floatingItems = new List<Screenspace3DSprite>(FloatingItem.TOTAL_ITEM_COUNT);

        private readonly Random random = new Random();

        private void MainMenu_OnLoad() {
            var menuTexture = Game.Content.Load<Texture2D>(MENU_SPRITE_ATLAS);
            fallingElf = new Screenspace3DSprite(menuTexture) {
                TextureSource = new Rectangle(0,0,41,29),
                PixelSmoothing = false,
                Depth = DepthConstants.Middle
            };
            waterOverlay = new TextureEntity(UITextures.Nothing) {
                Color = FOREGROUND_WATER_COLOR,
                Alpha = FOREGROUND_WATER_OPACITY,
                Scale = new Vector3(1f)
            };
            waterOverlay.Position = new Vector3(waterOverlay.Position.X,waterOverlay.Position.Y,DepthConstants.Foreground);
            waterOverlay.SetUVArea(0,32,16,16);
            fallingElf.OnUpdate += FallingElf_OnUpdate;

            Entities.Add(fallingElf);
            Entities.Add(waterOverlay);

            AddFloatingItems(menuTexture);
        }

        private void AddFloatingItems(Texture2D texture) {
            for(int i = 0;i<FloatingItem.TOTAL_ITEM_COUNT;i++) {
                var floatingItem = new FloatingItem(random,texture);
                floatingItems.Add(floatingItem);
                Entities.Add(floatingItem);
            }
        }

        private void FallingElf_OnUpdate(GameTime gameTime) {
            Rectangle bounds = Game.Viewport.Bounds;
            float scale = GetPixelScale();
            Vector2 center = new Vector2(bounds.Width * 0.5f,bounds.Height * (2f/3f));
            Vector2 size = fallingElf.TextureSource.Size.ToVector2() * scale;
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
