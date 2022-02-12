using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Shell;

namespace TwelveEngine.UI {

    /* Fundamentally, just a wrapper class */
    public class UIGameState:GameState {

        public UIGameState() {
            OnUnload += UIGameState_OnUnload;
            OnPreRender += UIGameState_OnPreRender;
            OnRender += render;
            OnUpdate += update;
        }

        private void UIGameState_OnPreRender(GameTime gameTime) {
            state?.PreRender(gameTime);
        }

        private void UIGameState_OnUnload() => state?.Unload();

        private UIState state;

        public UIState State {
            get => state;
            internal set {
                if(state == value) {
                    return;
                }
                if(state != null) {
                    state.Unload();
                }
                state = value;
                state.Load();
            }
        }

        private void render(GameTime gameTime) {
            Game.GraphicsDevice.Clear(Color.Black);
            state?.Render(gameTime);
        }

        private void update(GameTime gameTime) => state?.Update(gameTime);

        public static GameState Create(Action<UIState> generateState) {

            var gameState = new UIGameState();

            gameState.OnLoad += () => {
                var UI = new UIState(gameState.Game);

                generateState(UI);
                gameState.State = UI;

                UI.StartLayout();
            };

            return gameState;
        }
    }
}
