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

namespace Elves {
    public sealed class MainMenu:OrthoBackgroundState {

        const int FLOATING_ITEM_COUNT = 16;
        const string MENU_SPRITE_ATLAS = "Menu/falling-elf";

        const float ELF_Z = 5f;
        const float BACKGROUND_Z = 10f;
        const float FOREGROUND_Z = 8f;
        const int RANDOM_START_SEED = 27;

        private static readonly Color BACKGROUND_TOP_COLOR = Color.FromNonPremultiplied(0,225,89,255);
        private static readonly Color BACKGROUND_BOTTOM_COLOR = Color.Black;

        private const float FOREGROUND_WATER_OPACITY = 0.592f;
        private static Color FOREGROUND_WATER_COLOR => Color.FromNonPremultiplied(109,228,255,255);

        private float GetPixelScale() => Game.Viewport.Height / 60;

        public MainMenu():base(UITextures.Nothing) {
            SetBackgroundColors(BACKGROUND_TOP_COLOR,BACKGROUND_TOP_COLOR,BACKGROUND_BOTTOM_COLOR,BACKGROUND_BOTTOM_COLOR);
            OnLoad += MainMenu_OnLoad;
        }

        private Screenspace3DSprite fallingElf;
        private TextureEntity waterOverlay;

        private readonly List<Screenspace3DSprite> floatingItems = new List<Screenspace3DSprite>(FLOATING_ITEM_COUNT);

        private readonly Random random = new Random(RANDOM_START_SEED);

        private void MainMenu_OnLoad() {
            var menuTexture = Game.Content.Load<Texture2D>(MENU_SPRITE_ATLAS);
            fallingElf = new Screenspace3DSprite(menuTexture) {
                TextureSource = new Rectangle(0,0,41,29),
                PixelSmoothing = false,
                Z = ELF_Z
            };
            waterOverlay = new TextureEntity(UITextures.Nothing) {
                Color = FOREGROUND_WATER_COLOR,
                Alpha = FOREGROUND_WATER_OPACITY,
                Billboard = true,
                Scale = new Vector3(1f)
            };
            Background.Z = BACKGROUND_Z;
            waterOverlay.Position = new Vector3(waterOverlay.Position.X,waterOverlay.Position.Y,FOREGROUND_Z);
            waterOverlay.SetUVArea(0,32,16,16);
            fallingElf.OnUpdate += FallingElf_OnUpdate;

            Entities.Add(fallingElf);
            Entities.Add(waterOverlay);

            AddFloatingItems(menuTexture);
        }

        private void AddFloatingItems(Texture2D texture) {
            for(int i = 0;i<FLOATING_ITEM_COUNT;i++) {
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

        private class FloatingItem:Screenspace3DSprite {

            private const float MIN_DURATION = 35;
            private const float MAX_DURATION = 50;

            private const float DURATION_RANGE = MAX_DURATION - MIN_DURATION;

            private const float WIGGLE_RATE_MIN = 4;
            private const float WIGGLE_RATE_MAX = 8;

            private const float WIGGLE_RATE_RANGE = WIGGLE_RATE_MAX - WIGGLE_RATE_MIN;

            private const float WIGGLE_STRENGTH_MIN = 4;
            private const float WIGGLE_STRENGTH_MAX = 8;

            private const float WIGGLE_STRENGTH_RANGE = WIGGLE_STRENGTH_MAX - WIGGLE_STRENGTH_MIN;

            private static Rectangle[] FloatingItemSources = new Rectangle[] {
                new Rectangle(43,10,4,4)
            };

            public TimeSpan StartTime { get; set; }
            public TimeSpan Duration { get; set; }

            public float X { get; set; }

            public TimeSpan WiggleRate { get; set; }
            public float WiggleStrength { get; set; }

            private Rectangle GetRandomFloatingItemSource() {
                return FloatingItemSources[random.Next(0,FloatingItemSources.Length)];
            }

            private readonly Random random;

            public FloatingItem(Random random,Texture2D texture):base(texture) {
                this.random = random;
                OnUpdate += FloatingItem_OnUpdate;
                PixelSmoothing = false;
            }

            private bool firstTime = true;

            private void Reset() {
                StartTime = Game.Time.TotalGameTime;
                Duration = TimeSpan.FromSeconds(random.NextSingle() * DURATION_RANGE + MIN_DURATION);
                Z = 4f; //todo make depth work ugh
                X = random.NextSingle();
                if(firstTime) {
                    StartTime = StartTime.Subtract(Duration * random.NextSingle());
                    firstTime = false;
                }
                TextureSource = GetRandomFloatingItemSource();

                WiggleRate = TimeSpan.FromSeconds(random.NextSingle() * WIGGLE_RATE_RANGE + WIGGLE_RATE_MIN);
                WiggleStrength = WIGGLE_RATE_MIN + random.NextSingle() * WIGGLE_STRENGTH_RANGE + WIGGLE_STRENGTH_MIN;
            }

            private bool needsReset = true;

            private void FloatingItem_OnUpdate(GameTime gameTime) {
                if(needsReset) {
                    Reset();
                    needsReset = false;
                }
                float scale = (Owner as MainMenu).GetPixelScale();

                float t = (float)((gameTime.TotalGameTime - StartTime) / Duration);
                if(t < 0) {
                    t = 0;
                } else if(t > 1) {
                    t = 1;
                    needsReset = true;
                }


                Vector2 size = TextureSource.Size.ToVector2() * scale;
                float xValue = X * Game.Viewport.Width;

                Vector2 position = Vector2.Lerp(
                    new Vector2(xValue,Game.Viewport.Height),
                    new Vector2(xValue,-size.Y),t
                );

                var wiggleT = (float)(gameTime.TotalGameTime / WiggleRate % 1);
                var offset = MathF.Sin(MathF.PI * 2 * wiggleT);
                position.X += offset * WiggleStrength;

                Area = new VectorRectangle(position-new Vector2(size.X*0.5f,0f),size);
            }
        }
    }
}
