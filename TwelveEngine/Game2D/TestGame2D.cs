using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Game2D.Entities;
using nkast.Aether.Physics2D.Dynamics;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Game2D {
    public sealed class TestGame2D:GameState2D {

        private Texture2D texture;

        public TestGame2D() {
            OnLoad.Add(LoadSpriteSheet,EventPriority.First);
            OnLoad.Add(CreateEntities);
            OnUpdate.Add(CameraTracking);

            OnWriteDebug.Add(DebugTestGame);

            int width = 100;
            int height = 32;

            ushort brick = 1;

            Camera.TileInputSize = 16f;
            Camera.Scale = 10;

            Camera.Position = Vector2.Zero;

            TileDictionary[brick] = new TileData() {
                Color = Color.White,
                Source = new Rectangle(16,0,16,16),
                SpriteEffects = SpriteEffects.None
            };

            TileDictionary[2] = new TileData() {
                Color = Color.White,
                Source = new Rectangle(64,16,16,16),
                SpriteEffects = SpriteEffects.None
            };

            TileMap = new TileMap() { Width = width,Height = height,Data = new ushort[width*height] };
            for(int x = 0;x<TileMap.Width;x++) {
                TileMap.SetValue(x,height-1,brick);
            }

            Camera.MinX = 0;
            Camera.MaxX = width;
            Camera.MaxY = height;

            PhysicsWorld.Gravity = new Vector2(0f,5f);

            Body body = new Body() {
                FixedRotation = true,
                BodyType = BodyType.Static,
                SleepingAllowed = true,
                Mass = 100000,
                LocalCenter = new Vector2(0,0)
            };
            var fixture = body.CreateRectangle(width,1,1,new Vector2(width * 0.5f - 0.5f,0f));
            fixture.Friction = 20f;
            body.Position = new Vector2(0,height-1);

            PhysicsWorld.Add(body);
        }

        private void DebugTestGame(DebugWriter writer) {
            writer.ToTopLeft();
            writer.Write(_player.Position,"Player Position");
        }

        private Player _player;

        private void CreateEntities() {
            _player = new Player(texture,new Rectangle(64,0,16,16),Color.White) { Position = new Vector2(0,TileMap.Height-2) };
            Entities.Add(_player);
            //Entities.Add(new TestSquare(texture,new Rectangle(16,16,16,16),new Vector2(1,1),Color.White));
        }

        private void CameraTracking() {
            Camera.Position = _player.Position;
        }

        private void LoadSpriteSheet() {
            texture = Content.Load<Texture2D>("spritesheet");
            TileMapTexture = texture;
        }
    }
}
