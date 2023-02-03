using System;
using TwelveEngine;
using TwelveEngine.Effects;
using TwelveEngine.Shell;

namespace Elves.Scenes.Test {
    public sealed class ScrollingBackgroundTest:GameState {

        private readonly InfinityBackground infinityBackground = new() {
            Texture = Program.Textures.GiftPattern
        };

        public ScrollingBackgroundTest() {
            OnRender.Add(Render);
            OnLoad.Add(Load);
        }

        private void Load() {
            infinityBackground.Load(Content);
        }

        private void Render() {
            infinityBackground.Rotation = (float)(Now / TimeSpan.FromSeconds(4)) * 360;
            infinityBackground.Render(SpriteBatch,Viewport);
        }
    }
}
