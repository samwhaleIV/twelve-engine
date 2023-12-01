using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Entities;
using TwelveEngine.Shell.UI;

namespace John {
    public sealed class JohnCollectionGame:GameState2D {

        private static class Constants {
            public const int INPUT_TILE_SIZE = 16;
            public const float CAMERA_SCALE = 10f;

            public const float GRAVITY = 200f;

            public const string TILEMAP_TEXTURE = "spritesheet";
            public const string MAP_FILE = "john-collection.json";

            public const float PHYSICS_SIM_SCALE = 8f;
        }

        public JohnCollectionGame() {
            OnLoad.Add(LoadGame);
            OnUpdate.Add(CameraTracking);
            PhysicsScale = Constants.PHYSICS_SIM_SCALE;
        }

        private void CameraTracking() {
            Camera.Position = grabbyClaw.Position;
        }

        private GrabbyClaw grabbyClaw;

        private void LoadGame() {
            Camera.TileInputSize = Constants.INPUT_TILE_SIZE;
            Camera.Scale = Constants.CAMERA_SCALE;

            OnWriteDebug.Add(DrawPlayerPosition);

            TileMapTexture = Content.Load<Texture2D>(Constants.TILEMAP_TEXTURE);
            TileMap = TileMap.CreateFromJSON(Constants.MAP_FILE);

            SetCameraBounds();
            Camera.MaxY -= 0.75f;
            Camera.MinX += 0.25F;
            Camera.MaxX -= 0.25f;

            GenerateGenericTileDictionary();

            HashSet<short> nonCollidingTiles = new() { 81, 97, 113 };
            GenerateWorldCollision(tileValue => !nonCollidingTiles.Contains(tileValue));

            PhysicsWorld.Gravity = new Vector2(0,Constants.GRAVITY);

            grabbyClaw = new GrabbyClaw() {
                MinX = 22 / 16f,
                MaxX = TileMap.Width - 22 / 16F,
                MinY = 0.5f,
                MaxY = TileMap.Height - 20 / 16f
            };

            grabbyClaw.Position = new Vector2(13f,19f);
            Entities.Add(grabbyClaw);

            Entities.Add(new DebugDot() {
                Position = new Vector2(13f,19f)
            });
        }

        private void DrawPlayerPosition(DebugWriter writer) {
            writer.ToTopLeft();
            writer.Write(Camera.Position,"Camera Position");
            writer.Write(Camera.TileStart,"Tile Start");
            writer.Write(Camera.RenderOrigin,"Render Origin");
        }
    }
}
