using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game2D.Collision;

namespace TwelveEngine.Game2D.Entity.Types {
    public sealed class Player:MovingEntity2D {

        private Texture2D texture;

        public Player() {
            //Accel = Constants.Config.PlayerAccel;
            //Decay = Constants.Config.PlayerDeaccel;
            //MaxVelocity = Constants.Config.PlayerSpeed;
            OnUpdate += Player_OnUpdate;
            OnRender += Player_OnRender;
            OnLoad += Player_OnLoad;
        }

        private void Player_OnLoad() {
            texture = Game.Content.Load<Texture2D>(Constants.Config.PlayerImage);
        }

        private void Player_OnRender(GameTime gameTime) {
            if(!OnScreen()) {
                return;
            }
            Draw(texture,new Rectangle(0,0,16,16));
        }

        private void Player_OnUpdate(GameTime gameTime) => UpdateMovement(gameTime);

        public override Point GetMovementDelta() => Owner.Input.GetDelta2D();
        protected override int GetEntityType() => Entity2DType.Player;

        protected override CollisionData GetCollisionData() {
            return new CollisionData();
            //todo...
        }

        protected override void ResolveCollision(CollisionData collisionData) {
            //todo...
        }
    }
}
