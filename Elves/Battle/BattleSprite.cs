using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Game3D.Entity.Types;

namespace Elves.Battle {
    public class BattleSprite:TextureEntity {

        public const float SCREEN_EDGE_MARGIN = 0.01f;

        public BattleSprite(string textureName):base(textureName) {
            OnLoad += () => {

                float baseSize = Math.Max(Texture.Width,Texture.Height);
                float width = Texture.Width / baseSize, height = Texture.Height / baseSize;
                float halfWidth = width * 0.5f, halfHeight = height * 0.5f;

                Vector3 headToFeetOffset = new Vector3(0,halfHeight,0);

                TopLeft = new Vector3(-halfWidth,halfHeight,0f) + headToFeetOffset;
                BottomRight = new Vector3(halfWidth,-halfHeight,0f) + headToFeetOffset;
            };
            OnUpdate += gameTime => {
                Owner.Camera.Update(Game.Viewport.AspectRatio);
                Vector2 screenSize = Owner.Camera.OrthographicSize;
                float scale = screenSize.Y;
                Vector3 margin = new Vector3(0f,0.05f,0f);
                Position = new Vector3(Position.X,scale * SCREEN_EDGE_MARGIN,Position.Z);
                Scale = new Vector3(scale,scale,0) * (1f - SCREEN_EDGE_MARGIN * 2);
            };
        }
    }
}
