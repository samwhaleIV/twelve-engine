using TwelveEngine.Game3D.Entity.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine;
using Elves.Battle.Sprite.Animation;
using TwelveEngine.Game3D;

namespace Elves {
    public class Screenspace3DSprite:TextureEntity {

        public Screenspace3DSprite() => Initialize();

        public Screenspace3DSprite(string textureName) : base(textureName) => Initialize();
        public Screenspace3DSprite(Texture2D texture) : base(texture) => Initialize();

        public VectorRectangle Area { get; set; } = VectorRectangle.Zero; /* Space in pixels */
        public Rectangle TextureSource { get; set; }

        private void Initialize() {
            Billboard = true;
            OnPreRender += Screenspace3DSprite_OnPreRender;
        }

        private void Screenspace3DSprite_OnPreRender(GameTime gameTime) {
            SetUVArea(TextureSource);

            var orthoArea = Owner.Camera.OrthographicArea;
            var viewportSize = Owner.Game.Viewport.Bounds.Size.ToVector2();

            Vector2 scale = orthoArea.Size / viewportSize;

            Scale = new Vector3(Area.Size * scale,Scale.Z);

            TopLeft = new Vector3(0f,0f,0f);
            BottomRight = new Vector3(1f,-1f,0f);

            var position = orthoArea.TopLeft + new Vector2(0f,orthoArea.Height) + new Vector2(Area.X,-Area.Y) * scale;
            Position = new Vector3(position,Scale.Z);
        }
    }
}
