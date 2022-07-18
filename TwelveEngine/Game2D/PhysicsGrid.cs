using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;

namespace TwelveEngine.Game2D {
    public class PhysicsGrid:Grid2D {

        private readonly World _physicsWorld;

        public World PhysicsWorld => _physicsWorld;

        public PhysicsGrid() {
            _physicsWorld = new World();
            OnUpdate += PhysicsGrid_OnUpdate;
        }

        private void PhysicsGrid_OnUpdate(GameTime gameTime) {
            _physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        protected override void RenderGrid(GameTime gameTime) {
            RenderEntities(gameTime);
        }
    }
}
