using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game3D;
using TwelveEngine.Game3D.Entity.Types;
using Elves.Battle.Sprite.Elves;
using Elves.UI;
using System.Text;
using System;
using Elves.UI.Font;
using Elves.UI.Battle;

namespace Elves.Battle {
    public class BattleScene:OrthoBackgroundState {

        private BattleUI battleUI;

        public Color Tint { get; set; } = Color.White;

        public BattleScene(string backgroundImage = "Backgrounds/checkerboard") :base(backgroundImage) {

            OnLoad += BattleScene_OnLoad;

            ScrollingBackground = true;

            Mouse.OnPress += Mouse_OnPress;
            Mouse.OnRelease += Mouse_OnRelease;
            Mouse.OnMove += Mouse_OnMove;

            Input.OnAcceptDown += Input_OnAcceptDown;

            OnUpdateUI += BattleScene_OnUpdateUI;
            OnRender += BattleScene_OnRender;

            Tint = Color.Red;
            SetBackgroundColor(Color.Red);
        }

        private void BattleScene_OnLoad() {
            battleUI = new BattleUI(Game);
            Entities.Add(new HarmlessElf());
        }

        private void Input_OnAcceptDown() {
            battleUI.TestButtonAnimation();
        }

        private void Mouse_OnRelease(Point point) {
            battleUI.MouseRelease(point.X,point.Y);
        }

        private void Mouse_OnPress(Point point) {
            battleUI.MousePress(point.X,point.Y);
        }

        private void Mouse_OnMove(Point point) {
            battleUI.MouseMoved(point.X,point.Y);
        }

        private void BattleScene_OnUpdateUI(GameTime gameTime) {
            battleUI.UpdateUI(GetUIScale());
        }

        private void BattleScene_OnRender(GameTime gameTime) {
            battleUI.Render(GetUIScale(),Tint,Game.SpriteBatch);
        }
    }
}
