using TwelveEngine.Effects;
using TwelveEngine.Shell;
using Microsoft.Xna.Framework.Graphics;

namespace Elves.Scenes.Test {
    public sealed class DVDCornerTest:GameState {

        private readonly WillTheDVDHitTheCornerSimulator bouncingSimulator = new() {
            DistancePerSecond = 0.25f,
            MinimumRandomVelocity = 0.6f,
        };

        public DVDCornerTest() {
            OnUpdate.Add(Update);
            OnRender.Add(Render);
        }

        private void Render() {
            SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);
            bouncingSimulator.Render(SpriteBatch,Program.Textures.Warning,Viewport.Height * 0.025f,Viewport.Bounds);
            SpriteBatch.End();
        }

        private void Update() {
            bouncingSimulator.Update(Now);
        }
    }
}
