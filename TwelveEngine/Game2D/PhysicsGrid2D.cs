using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D {
    public class PhysicsGrid2D:Grid2D {

        private readonly World _physicsWorld;

        public World PhysicsWorld => _physicsWorld;

        public PhysicsGrid2D() {
            _physicsWorld = new World(Vector2.Zero);
            OnUpdate += PhysicsGrid_OnUpdate;
        }

        private void PhysicsGrid_OnUpdate(GameTime gameTime) {
            _physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        protected override void RenderGrid(GameTime gameTime) {
            Game.SpriteBatch.Begin(SpriteSortMode.Deferred);
            RenderEntities(gameTime);
            Game.SpriteBatch.End();
        }
    }
}
