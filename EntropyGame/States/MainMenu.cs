using TwelveEngine.Shell.States;
using Microsoft.Xna.Framework;

namespace EntropyGame.States {
    internal class MainMenu:InputGameState {

        public MainMenu() {
            OnLoad += MainMenu_OnLoad;
            OnRender += MainMenu_OnRender;
        }

        private void MainMenu_OnRender(GameTime gameTime) {
            Game.GraphicsDevice.Clear(Color.Black);

            
        }

        private void MainMenu_OnLoad() {
            
        }
    }
}
