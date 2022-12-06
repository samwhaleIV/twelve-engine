using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game3D;
using TwelveEngine.Game3D.Entity.Types;
using Elves.Battle.Sprite.Elves;
using Elves.UI;
using System.Text;
using System;
using Elves.UI.Font;

namespace Elves.Battle {
    public class BattleScene:OrthoBackgroundState {

        public BattleScene(string backgroundImage = "Backgrounds/checkerboard") :base(backgroundImage) {

            ScrollingBackground = true;

            SetBackgroundColors(Color.Red,Color.Purple,Color.Red,Color.Purple);

            var nineGrid = new NineGrid();

            OnLoad += () => {
                Entities.Add(new HarmlessElf());
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
                nineGrid.Draw(Game.SpriteBatch);

                Game.SpriteBatch.End();

                var font = Fonts.DefaultFont;
                font.Begin(Game.SpriteBatch);
                font.DrawCentered(stringBuilder,Game.Viewport.Bounds.Center,scale,Color.White);
                font.End();
            };
        }
    }
}
