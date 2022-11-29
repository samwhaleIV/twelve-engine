using TwelveEngine.Shell.States;
using Microsoft.Xna.Framework;
using Elves.BattleSequencer;

namespace Elves.States {
    public sealed class BattleSequencer:InputGameState {

        public BattleSequencer() {
            OnLoad += BattleSequencer_OnLoad;
            OnRender += BattleSequencer_OnRender;
            
            //sequencer = new UVSequencer();
        }

        private void BattleSequencer_OnRender(GameTime gameTime) {
            Game.GraphicsDevice.Clear(Color.Black);

            
        }

        private void BattleSequencer_OnLoad() {
            
        }
    }
}
