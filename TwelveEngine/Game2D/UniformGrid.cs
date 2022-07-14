using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Game2D.Collision.Poly;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D {
    public class UniformGrid:Grid2D {

        private readonly PolyCollision _polyCollision;
        public PolyCollision PolyCollision => _polyCollision;

        public UniformGrid() {
            _polyCollision = new PolyCollision();
            Collision = _polyCollision;         
        }

        protected override void RenderGrid(GameTime gameTime) {
            Game.SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            RenderEntities(gameTime);
            Game.SpriteBatch.End();
        }
    }
}
