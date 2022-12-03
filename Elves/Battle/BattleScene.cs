using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game3D;
using TwelveEngine.Game3D.Entity.Types;
using Elves.Battle.Sprite.Elves;
using Elves.UI;
using System.Text;

namespace Elves.Battle {
    public class BattleScene:World {

        public double BackgroundScrollTime { get; set; } = 60d;

        private readonly UVSpriteFont spriteFont;

        public UVSpriteFont SpriteFont => spriteFont;

        public BattleScene(string backgroundImage) {
            spriteFont = new UVSpriteFont();
            ClearColor = Color.Black;
            SamplerState = SamplerState.PointClamp;
            OnLoad += () => {
                spriteFont.Load(Game);
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
                SpriteFont.Begin();
                SpriteFont.DrawCentered(stringBuilder,Game.Viewport.Bounds.Center,2,Color.White);
                SpriteFont.End();
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
