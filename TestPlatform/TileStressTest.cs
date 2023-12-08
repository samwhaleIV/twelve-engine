using TwelveEngine;
using TwelveEngine.Game2D.Entities;
using TwelveEngine.Shell.UI;
using TwelveEngine.Input;
using TwelveEngine.Game2D;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TestPlatform {
    public sealed class TileStressTest:GameState2D {

        public TileStressTest() {
            OnLoad.Add(LoadGame);
            OnUpdate.Add(SetCameraPosition);
        }

        private void LoadGame() {
            Camera.TileInputSize = 2;
            Camera.Scale = 4;

            OnWriteDebug.Add(DrawDebugText);

            TileMapTexture = Content.Load<Texture2D>("spritesheet2");

            int testSize = 1000;
            int tileCount = testSize * testSize;
            var tileData = new short[1,tileCount];

            GenerateGenericTileSet();

            for(int i = 0;i<tileCount;i++) {
                tileData[0,i] = (short)(i % TileSet.Length);
            }

            var tileMap = new TileMap() {
                Width = testSize,
                Height = testSize,
                LayerCount = 1,
                Data = tileData
            };
            TileMap = tileMap;
            SetCameraBounds();

            PhysicsWorld.Enabled = false;
        }

        private void SetCameraPosition() {
            Camera.Position = TileMap.Size.ToVector2() * 0.5f;
        }

        private void DrawDebugText(DebugWriter writer) {
            writer.ToTopLeft();
            writer.Write("Hello, world!");
        }
    }
}
