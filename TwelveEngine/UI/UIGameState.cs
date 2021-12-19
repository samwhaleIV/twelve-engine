using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.UI {

    /* Fundamentally, just a wrapper class */
    public class UIGameState:GameState {

        public UIGameState() {
            OnUnload += UIGameState_OnUnload;
            OnPreDraw += UIGameState_OnPreDraw;
        }

        private void UIGameState_OnPreDraw(GameTime gameTime) {
            state?.PreRender(gameTime);
        }

        private void UIGameState_OnUnload() => state?.Unload();

        private UIState state;

        public UIState State {
            get => state;
            internal set {
                if(state != null) {
                    state.Unload();
                }
                state = value;
            }
        }

        public override void Draw(GameTime gameTime) {
            Game.GraphicsDevice.Clear(Color.Black);
            state?.Render(gameTime);
        }

        public override void Update(GameTime gameTime) => state?.Update(gameTime);

        public static GameState Create(Action<UIState> generateState) {

            var gameState = new UIGameState();

            gameState.OnLoad += () => {
                var UI = new UIState(gameState.Game);
                gameState.State = UI;
                generateState(UI);
                UI.UpdateCache();
                UI.Load();
                UI.StartLayout();
            };

            return gameState;
        }
    }
}
