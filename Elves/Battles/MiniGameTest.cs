using System;
using System.Threading.Tasks;
using Elves.Battle;
using Elves.ElfData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Shell;

namespace Elves.Battles {
    public sealed class MiniGameTest:BattleScript {

        public class TestMiniGame:ResultMiniGame<bool> {

            public override void Activate() {
                GameState.Impulse.Router.OnCancelDown += Router_OnCancelDown;
            }

            public override void Deactivate() {
                GameState.Impulse.Router.OnCancelDown -= Router_OnCancelDown;
            }

            private void Router_OnCancelDown() {
                SetResult(true);
            }

            public override void Render(SpriteBatch spriteBatch,int width,int height) {
                spriteBatch.Begin();
                spriteBatch.Draw(Program.Textures.Nothing,new Rectangle(0,0,width,height),Color.Red);
                spriteBatch.End();
            }
        }

        private readonly TestMiniGame _miniGame = new();

        public override async Task<BattleResult> Main() {
            while(true) {
                await GetButton("Mini game!");
                await ShowMiniGame(_miniGame);
            }
            return BattleResult.PlayerWon;
        }

        public override async Task Exit(BattleResult battleResult) {
            if(battleResult == BattleResult.PlayerWon) {
                SetTag("Good job.");
                await Continue();
                Actor.Kill();
            } else {
                SetTag("I lied.");
                await Continue();
                Player.Kill();
            }
            await base.Exit(battleResult);
        }
    }
}
