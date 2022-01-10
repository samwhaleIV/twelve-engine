using TwelveEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ElfGame {
    internal class FacialAnimationViewer:GameState {

        private Texture2D faceTexture;

        public FacialAnimationViewer() {
            OnLoad += FacialAnimationViewer_OnLoad;
            OnRender += FacialAnimationViewer_OnRender;
        }

        private void FacialAnimationViewer_OnLoad() {
            Input.OnAcceptDown += Input_OnAcceptDown;
            OnUnload += () => Input.OnAcceptDown -= Input_OnAcceptDown;
            faceTexture = Game.Content.Load<Texture2D>("elf-face");

        }

        private void Input_OnAcceptDown() {
            //Randomize state
        }

        private void FacialAnimationViewer_OnRender(GameTime obj) {
            Game.SpriteBatch.Begin(SpriteSortMode.Deferred);


            Game.SpriteBatch.End();
        }
    }
}
