using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game3D;
using TwelveEngine.Game3D.Entity.Types;
using Elves.Battle.Sprite.Elves;
using Elves.UI;
using System.Text;
using System;

namespace Elves.Battle {
    public class BattleScene:World {

        public double BackgroundScrollTime { get; set; } = 60d;

        private const int MAX_SCALE = 8;

        private int GetUIScale() {
            int screenHeight = Game.Viewport.Height;
            /* 2160 (4K) / 8 = 270 */
            return Math.Min(Math.Max(screenHeight / 270 - 2,1),MAX_SCALE);
        }

        private readonly UVSpriteFont spriteFont;

        public UVSpriteFont SpriteFont => spriteFont;

        public BattleScene(string backgroundImage) {
            ClearColor = Color.Black;
            SamplerState = SamplerState.PointClamp;
            var nineGrid = new NineGrid();
            OnLoad += () => {
                var camera = new AngleCamera() {
                    NearPlane = 0.1f,
                    FarPlane = 20f,
                    FieldOfView = 75f,
                    Orthographic = true,
                    Angle = new Vector2(0f,180f),
                    Position = new Vector3(0f,0f,10f)
                };
                Camera = camera;
                var backgroundEntity = new TextureEntity(backgroundImage) {
                    Name = "ScrollingBackground",
                    PixelSmoothing = true,
                    Billboard = true,
                    Scale = new Vector3(1f)
                };

                Entities.Add(backgroundEntity);
                Entities.Add(new HarmlessElf());
            };
            OnUpdate += gameTime => {
                UpdateBackground(gameTime);
            };
            var stringBuilder = new StringBuilder("Hello, world!");
            OnRender += gameTime => {
                RenderEntities(gameTime);

                Game.SpriteBatch.Begin(SpriteSortMode.Deferred,BlendState.NonPremultiplied,SamplerState.PointClamp);

                int scale = GetUIScale();

                Point size = new Point(250,100) * new Point(scale);
                Point halfSize = size / new Point(2);
                nineGrid.Area = new Rectangle(Game.Viewport.Bounds.Center-halfSize,size);
                nineGrid.Scale = scale * 4;
                nineGrid.Draw();

                Game.SpriteBatch.End();

                Fonts.UIFont.Begin(Game.SpriteBatch);
                SpriteFont.DrawCentered(stringBuilder,Game.Viewport.Bounds.Center,scale,Color.White);
                Fonts.UIFont.End();
            };
        }

        private void UpdateBackground(GameTime gameTime) {
            TextureEntity background = Entities.Get<TextureEntity>("ScrollingBackground");
            background.SetColors(Color.Red,Color.Purple,Color.Red,Color.Purple);
            double scrollT = gameTime.TotalGameTime.TotalSeconds / BackgroundScrollTime % 1d;
            background.UVOffset = new Vector2((float)scrollT,0f);
        }
    }
}
