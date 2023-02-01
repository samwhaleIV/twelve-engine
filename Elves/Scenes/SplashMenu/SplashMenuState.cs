﻿using Microsoft.Xna.Framework;
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

        private bool capturingPlayButton = false;

        public SplashMenuState():base(Program.Textures.Nothing,true) {
            Name = "Elves Splash Menu";
            SetBackgroundColor(BACKGROUND_TOP_COLOR,BACKGROUND_TOP_COLOR,BACKGROUND_BOTTOM_COLOR,BACKGROUND_BOTTOM_COLOR);
            OnLoad += SplashMenuState_OnLoad;
            Mouse.Router.OnRelease += Mouse_OnRelease;
            Mouse.Router.OnPress += Router_OnPress;
            Impulse.Router.OnAcceptDown += Input_OnAcceptDown;
            OnUpdate += SplashMenuState_OnUpdate;
        }

        private void Router_OnPress() {
            capturingPlayButton = MouseOnPlayButton;
        }

        private void SplashMenuState_OnUpdate() {
            if(IsTransitioning) {
                CustomCursor.State = CursorState.Default;
                return;
            }
            CustomCursor.State = capturingPlayButton ? (MouseOnPlayButton ? CursorState.Pressed : CursorState.Default) : (MouseOnPlayButton ? CursorState.Interact : CursorState.Default);
        }

        private void Input_OnAcceptDown() {
            if(Mouse.CapturingLeft) {
                return;
            }
            EndScene(ExitValue.None);
        }

        private bool MouseOnPlayButton => playButton.Area.Contains(Mouse.Position);

        private void Mouse_OnRelease() {
            capturingPlayButton = false;
            if(!MouseOnPlayButton) {
                return;
            }
            EndScene(ExitValue.None);
        }

        private Screenspace3DSprite fallingElf;
        private TextureEntity waterOverlay;
        private Screenspace3DSprite nameBadge;
        private Screenspace3DSprite playButton;

        private readonly List<Screenspace3DSprite> floatingItems = new(FloatingItem.TOTAL_ITEM_COUNT);

        private readonly Random random = new();

        private void SplashMenuState_OnLoad() {
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
            playButton = new Screenspace3DSprite(menuTexture) {
                TextureSource = new Rectangle(0,78,38,20),
                PixelSmoothing = false,
                Depth = Constants.Depth.SuperForeground
            };
            waterOverlay.Position = new Vector3(waterOverlay.Position.X,waterOverlay.Position.Y,Constants.Depth.Foreground);
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

        private void PlayButton_OnUpdate() {
            Rectangle bounds = Viewport.Bounds;
            float scale = GetUIScale() * PLAY_BUTTON_SCALE;
            Vector2 size = playButton.TextureSource.Size.ToVector2() * scale;
            Vector2 center = new(bounds.Width * 0.5f - size.X * 0.5f,bounds.Height * (2/3f) - size.Y * 0.5f);
            playButton.Area = new FloatRectangle(center,size);
        }

        private void NameBadge_OnUpdate() {
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

        private void FallingElf_OnUpdate() {
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
