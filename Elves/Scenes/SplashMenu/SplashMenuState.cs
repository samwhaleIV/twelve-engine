using Microsoft.Xna.Framework;
using TwelveEngine.Game3D.Entity.Types;
using TwelveEngine;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TwelveEngine.Shell;

namespace Elves.Scenes.SplashMenu {
    public sealed class SplashMenuState:OrthoBackgroundScene {
        private const float PLAY_BUTTON_SCALE = 0.75f;

        private static readonly Color BACKGROUND_TOP_COLOR = Color.FromNonPremultiplied(0,225,89,255);
        private static readonly Color BACKGROUND_BOTTOM_COLOR = Color.Black;

        private const float FOREGROUND_WATER_OPACITY = 0.592f;
        private static Color FOREGROUND_WATER_COLOR => Color.FromNonPremultiplied(109,228,255,255);


        public SplashMenuState():base(Program.Textures.Nothing,true) {
            Name = "Splash Menu";
            SetBackgroundColor(BACKGROUND_TOP_COLOR,BACKGROUND_TOP_COLOR,BACKGROUND_BOTTOM_COLOR,BACKGROUND_BOTTOM_COLOR);
            OnLoad.Add(Load);
            UI = new SplashMenuUI(this);
        }

        private Screenspace3DSprite fallingElf;
        private TextureEntity waterOverlay;
        private Screenspace3DSprite nameBadge;
        public Screenspace3DSprite PlayButton;
        public SplashMenuUI UI { get; private set; }

        private readonly List<Screenspace3DSprite> floatingItems = new(FloatingItem.TOTAL_ITEM_COUNT);

        private readonly Random random = new();

        private void Load() {
            var menuTexture = Program.Textures.Drowning;
            fallingElf = new Screenspace3DSprite(menuTexture) {
                TextureSource = new Rectangle(0,0,41,29),
                PixelSmoothing = false,
                Depth = Constants.Depth.Middle
            };
            waterOverlay = new TextureEntity(Program.Textures.Nothing) {
                Color = FOREGROUND_WATER_COLOR,
                Alpha = FOREGROUND_WATER_OPACITY,
                Scale = new Vector3(1f)
            };
            nameBadge = new Screenspace3DSprite(menuTexture) {
                TextureSource = new Rectangle(41,0,54,20),
                PixelSmoothing = false,
                Depth = Constants.Depth.SuperForeground
            };
            PlayButton = new Screenspace3DSprite(menuTexture) {
                TextureSource = new Rectangle(0,78,38,20),
                PixelSmoothing = false,
                Depth = Constants.Depth.SuperForeground
            };
            waterOverlay.Position = new Vector3(waterOverlay.Position.X,waterOverlay.Position.Y,Constants.Depth.Foreground);
            waterOverlay.SetUVArea(0,32,16,16);
            fallingElf.OnUpdate += UpdateFallingElfArea;
            nameBadge.OnUpdate += UpdateNameBadgeArea;
            PlayButton.OnUpdate += UpdatePlayButtonArea;

            Entities.Add(fallingElf);
            Entities.Add(waterOverlay);
            Entities.Add(nameBadge);
            Entities.Add(PlayButton);
            AddFloatingItems(menuTexture);
        }

        private void UpdatePlayButtonArea() {
            Rectangle bounds = Viewport.Bounds;
            float scale = GetUIScale() * PLAY_BUTTON_SCALE;
            Vector2 size = PlayButton.TextureSource.Size.ToVector2() * scale;
            Vector2 center = new(bounds.Width * 0.5f - size.X * 0.5f,bounds.Height * (2/3f) - size.Y * 0.5f);
            PlayButton.Area = new FloatRectangle(center,size);
        }

        private void UpdateNameBadgeArea() {
            Rectangle bounds = Viewport.Bounds;
            float scale = GetUIScale();
            Vector2 size = nameBadge.TextureSource.Size.ToVector2() * scale;

            Vector2 center = new(bounds.Width * 0.5f - size.X * 0.5f,bounds.Height * (1/3f) - size.Y * 0.5f);
            nameBadge.Area = new FloatRectangle(center,size);
        }

        private void AddFloatingItems(Texture2D texture) {
            for(int i = 0;i<FloatingItem.TOTAL_ITEM_COUNT;i++) {
                var floatingItem = new FloatingItem(this,random,texture);
                floatingItems.Add(floatingItem);
                Entities.Add(floatingItem);
            }
        }

        private void UpdateFallingElfArea() {
            Rectangle bounds = Viewport.Bounds;
            float scale = GetUIScale();
            Vector2 size = fallingElf.TextureSource.Size.ToVector2() * scale;
            Vector2 center = new(bounds.Width * 0.5f - size.X * 0.5f,bounds.Height * (2/3f) - size.Y * 0.5f);
            var t = (float)(Now / TimeSpan.FromSeconds(8) % 1);
            var t2 = (float)(Now / TimeSpan.FromSeconds(16) % 1);
            var offset = MathF.Sin(MathF.PI * 2 * t);
            var offset2 = MathF.Cos(MathF.PI * 2 * t2);
            center.X += (offset * 8f) / FloatingItem.WIGGLE_BASE_SCALE * scale;
            center.Y += (offset2 * 4f) / FloatingItem.WIGGLE_BASE_SCALE * scale;
            fallingElf.Area = new FloatRectangle(center,size);
        }
    }
}
