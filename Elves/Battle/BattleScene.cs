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
using System.Xml.Linq;

namespace Elves.Battle {
    public class BattleScene:OrthoBackgroundState {

        private BattleUI battleUI;

        public readonly UserRenderData playerRenderData = new UserRenderData();
        public readonly UserRenderData targetRenderData = new UserRenderData();

        private void SetupUserData() {
            playerRenderData.Name = new StringBuilder("You".ToUpperInvariant());
            playerRenderData.Health = 0.75f;
            playerRenderData.Color = Color.White;

            targetRenderData.Name = new StringBuilder("Harmless Elf".ToUpperInvariant());
            targetRenderData.Health = 0.75f;
            targetRenderData.Color = Color.Red;

            SetBackgroundColor(targetRenderData.Color);
        }

        public BattleScene(string backgroundImage = "Backgrounds/checkerboard") :base(backgroundImage) {
            OnLoad += BattleScene_OnLoad;

            ScrollingBackground = true;

            Mouse.OnPress += Mouse_OnPress;
            Mouse.OnRelease += Mouse_OnRelease;
            Mouse.OnMove += Mouse_OnMove;

            OnUpdateUI += BattleScene_OnUpdateUI;
            OnRender += BattleScene_OnRender;

            SetupUserData();
        }

        private void BattleUI_OnActionButtonClick(int ID) {
            var button = battleUI.GetActionButton(ID);
            button.SetState(Now,button.State.GetOffscreen());
        }

        private void BattleScene_OnLoad() {
            battleUI = new BattleUI(Game);
            battleUI.OnActionButtonClick += BattleUI_OnActionButtonClick;
            var elf = new HarmlessElf();
            elf.SetSpritePosition(Now,Sprite.SpritePosition.Left,() => { });
            Entities.Add(elf);
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

        private void BattleScene_OnUpdateUI() {
            battleUI.Update((int)GetUIScale());
        }

        private void BattleScene_OnRender() {
            battleUI.Render(Game.SpriteBatch,playerRenderData,targetRenderData);
        }
    }
}
