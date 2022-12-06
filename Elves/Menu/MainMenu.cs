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
using Elves.UI.Font;
using System.Text;

namespace Elves.Menu {
    public sealed class MainMenu:OrthoBackgroundState {

        private const string MENU_SPRITE_ATLAS = "Menu/falling-elf";

        private const float VERTICAL_SCALE_DIVIDEND = 70f;
        private const float PLAY_BUTTON_SCALE = 0.75f;

        private static readonly Color BACKGROUND_TOP_COLOR = Color.FromNonPremultiplied(0,225,89,255);
        private static readonly Color BACKGROUND_BOTTOM_COLOR = Color.Black;

        private const float FOREGROUND_WATER_OPACITY = 0.592f;
        private static Color FOREGROUND_WATER_COLOR => Color.FromNonPremultiplied(109,228,255,255);

        public float GetMenuItemScale() => Game.Viewport.Height / VERTICAL_SCALE_DIVIDEND;

        public MainMenu(bool debug3D = false):base(UITextures.Nothing,true,debug3D) {
            SetBackgroundColors(BACKGROUND_TOP_COLOR,BACKGROUND_TOP_COLOR,BACKGROUND_BOTTOM_COLOR,BACKGROUND_BOTTOM_COLOR);
            OnLoad += MainMenu_OnLoad;
        }

        private Screenspace3DSprite fallingElf;
        private TextureEntity waterOverlay;
        private Screenspace3DSprite nameBadge;
        private Screenspace3DSprite playButton;

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
            nameBadge = new Screenspace3DSprite(menuTexture) {
                TextureSource = new Rectangle(41,0,54,20),
                PixelSmoothing = false,
                Depth = DepthConstants.SuperForeground
            };
            playButton = new Screenspace3DSprite(menuTexture) {
                TextureSource = new Rectangle(95,0,38,20),
                PixelSmoothing = false,
                Depth = DepthConstants.SuperForeground
            };
            waterOverlay.Position = new Vector3(waterOverlay.Position.X,waterOverlay.Position.Y,DepthConstants.Foreground);
            waterOverlay.SetUVArea(0,32,16,16);
            fallingElf.OnUpdate += FallingElf_OnUpdate;
            nameBadge.OnUpdate += NameBadge_OnUpdate;
            playButton.OnUpdate += PlayButton_OnUpdate;

            Entities.Add(fallingElf);
            Entities.Add(waterOverlay);
            Entities.Add(nameBadge);
            Entities.Add(playButton);
            AddFloatingItems(menuTexture);

        }

        private void PlayButton_OnUpdate(GameTime gameTime) {
            Rectangle bounds = Game.Viewport.Bounds;
            float scale = GetMenuItemScale() * PLAY_BUTTON_SCALE;
            Vector2 size = playButton.TextureSource.Size.ToVector2() * scale;
            Vector2 center = new Vector2(bounds.Width * 0.5f - size.X * 0.5f,bounds.Height * (2/3f) - size.Y * 0.5f);
            playButton.Area = new VectorRectangle(center,size);
        }

        private void NameBadge_OnUpdate(GameTime gameTime) {
            Rectangle bounds = Game.Viewport.Bounds;
            float scale = GetMenuItemScale();
            Vector2 size = nameBadge.TextureSource.Size.ToVector2() * scale;

            Vector2 center = new Vector2(bounds.Width * 0.5f - size.X * 0.5f,bounds.Height * (1/3f) - size.Y * 0.5f);
            nameBadge.Area = new VectorRectangle(center,size);
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
            float scale = GetMenuItemScale();
            Vector2 size = fallingElf.TextureSource.Size.ToVector2() * scale;
            Vector2 center = new Vector2(bounds.Width * 0.5f - size.X * 0.5f,bounds.Height * (2/3f) - size.Y * 0.5f);
            var t = (float)(gameTime.TotalGameTime / TimeSpan.FromSeconds(8) % 1);
            var t2 = (float)(gameTime.TotalGameTime / TimeSpan.FromSeconds(16) % 1);
            var offset = MathF.Sin(MathF.PI * 2 * t);
            var offset2 = MathF.Cos(MathF.PI * 2 * t2);
            center.X += (offset * 8f) / FloatingItem.WIGGLE_BASE_SCALE * scale;
            center.Y += (offset2 * 4f) / FloatingItem.WIGGLE_BASE_SCALE * scale;
            fallingElf.Area = new VectorRectangle(center,size);
        }
    }
}
