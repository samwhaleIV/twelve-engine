using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.UI {
    public class UIGameState:GameState {

        public UIGameState() => OnUnload += UIGameState_OnUnload;
        private void UIGameState_OnUnload() => state?.Unload();

        private UIState state;

        public UIState State {
            get => state;
            set {
                if(state != null) {
                    state.Unload();
                }
                state = value;
                if(!IsLoaded) {
                    state.Load();
                }
            }
        }

        public override void Draw(GameTime gameTime) => state?.Draw(gameTime);
        public override void Update(GameTime gameTime) => state?.Update();

        public static GameState Create(Action<UIState> generateState) {

            var gameState = new UIGameState();

            gameState.OnLoad += () => {
                var UI = new UIState(gameState.Game);
                gameState.State = UI;
                generateState(UI);
                UI.Load();
                UI.Root.StartLayout();
                UI.UpdateCache();
            };

            return gameState;
        }
    }
}
