using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TwelveEngine.Game3D.Entity.Types;
using TwelveEngine.Serial;

namespace Elves.Battle {
    public class BattleSprite:TextureEntity {

        public const float SCREEN_EDGE_MARGIN = 0.01f;
        public const double POSITION_CHANGE_DURATION = 0.1f;
        public const double MAX_SCREEN_WIDTH = 2f;
        public const float CENTER_Z = 5f;

        private TimeSpan positionChangeStart = TimeSpan.Zero;
        private SpritePosition? oldPosition = null;
        private SpritePosition spritePosition = SpritePosition.Center;

        public SpritePosition SpritePosition {
            get => spritePosition;
            set {
                spritePosition = value;
                oldPosition = null;
            }
        }

        private static readonly Dictionary<SpritePosition,Vector3> positionTable = new Dictionary<SpritePosition,Vector3>() {
            {SpritePosition.Left,new Vector3(-0.4f,0f,CENTER_Z - 1f)},
            {SpritePosition.Right,new Vector3(0.4f,0f,CENTER_Z - 1f)},
            {SpritePosition.Center,new Vector3(-0f,0f,CENTER_Z)},
            {SpritePosition.CenterLeft,new Vector3(-0.2f,0f,CENTER_Z)},
            {SpritePosition.CenterRight,new Vector3(0.2f,0f,CENTER_Z)},
        };

        private Vector3 GetPosition(SpritePosition spritePosition,float screenWidth) {
            var position = positionTable[spritePosition];
            float aspectRatio = Game.Viewport.AspectRatio;
            if(aspectRatio < 1) {
                position.X = position.X * screenWidth;
            } else {
                position.X = position.X * screenWidth / aspectRatio;
            }
            return position;
        }

        public void SetSpritePosition(GameTime gameTime,SpritePosition spritePosition,Action callback) {
            oldPosition = this.spritePosition;
            this.spritePosition = spritePosition;
            positionChangeStart = gameTime.TotalGameTime;
            Owner.SetTimeout(() => {
                callback();
                oldPosition = null;
                positionChangeStart = TimeSpan.Zero;
            },TimeSpan.FromSeconds(POSITION_CHANGE_DURATION));
        }

        public SpritePosition GetSpritePosition() => spritePosition;

        private Rectangle area;

        public Rectangle Area {
            get => area;
            set {
                area = value;
                UVTopLeft = new Vector2(
                    value.X / (float)Texture.Width,
                    value.Y / (float)Texture.Height
                );
                UVBottomRight = new Vector2(
                    value.Right / (float)Texture.Width,
                    value.Bottom / (float)Texture.Height
                );
            }
        }

        public BattleSprite(string textureName,int x,int y,int width,int height,int baseHeight) :base(textureName) {
            area = new Rectangle(x,y,width,height);
            OnImport += BattleSprite_OnImport;
            OnExport += BattleSprite_OnExport;
            OnLoad += () => {
                Area = area;
                float baseSize = baseHeight;
                float width = area.Width / baseSize, height = area.Height / baseSize;
                float halfWidth = width * 0.5f, halfHeight = height * 0.5f;
                Vector3 baseSizeOffset = new Vector3(0f,(1f-height)*-0.5f,0f);
                TopLeft = new Vector3(-halfWidth,halfHeight,0f) + baseSizeOffset;
                BottomRight = new Vector3(halfWidth,-halfHeight,0f) + baseSizeOffset;
            };
            OnUpdate += gameTime => {
                Vector2 screenSize = Owner.Camera.OrthographicSize;
                float scale = screenSize.Y;
                Vector3 margin = new Vector3(0f,0.05f,0f);
                Scale = new Vector3(scale,scale,0) * (1f - SCREEN_EDGE_MARGIN);
                if(!oldPosition.HasValue) {
                    Position = GetPosition(spritePosition,screenSize.X);
                    return;
                }
                Vector3 startPosition = GetPosition(oldPosition.Value,screenSize.X);
                Vector3 newPosition = GetPosition(spritePosition,screenSize.X);
                float positionT = (float)((gameTime.TotalGameTime - positionChangeStart).TotalSeconds / POSITION_CHANGE_DURATION);
                if(positionT < 0f) {
                    positionT = 0f;
                } else if(positionT > 1f) {
                    positionT = 1f;
                }
                Position = Vector3.Lerp(startPosition,newPosition,positionT);
            };
        }

        private void BattleSprite_OnExport(SerialFrame frame) {
            frame.Set(Area);
        }
        private void BattleSprite_OnImport(SerialFrame frame) {
            area = frame.GetRectangle();
        }
    }
}
