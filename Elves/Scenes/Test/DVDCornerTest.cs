using Elves.FX;
using TwelveEngine.Shell;
using Microsoft.Xna.Framework.Graphics;

namespace Elves.Scenes.Test {
    public sealed class DVDCornerTest:GameState {

        private readonly WillTheDVDHitTheCornerSimulator bouncingSimulator = new() {
            DistancePerSecond = 0.25f,
            MinimumRandomVelocity = 0.6f,
        };

        public DVDCornerTest() {
            OnUpdate += DVDCornerTest_OnUpdate;
            OnRender += DVDCornerTest_OnRender;
        }

        private void DVDCornerTest_OnRender() {
            SpriteBatch spriteBatch = Game.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);
            bouncingSimulator.Render(spriteBatch,Program.Textures.Warning,Game.Viewport.Height * 0.025f,Game.Viewport.Bounds);
            spriteBatch.End();
        }

        private void DVDCornerTest_OnUpdate() {
            bouncingSimulator.Update(Now);
        }
    }
}
