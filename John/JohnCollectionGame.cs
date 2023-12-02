using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine;
using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Entities;
using TwelveEngine.Shell.UI;

namespace John {
    public sealed class JohnCollectionGame:GameState2D {

        private GrabbyClaw grabbyClaw;
        private readonly PoolOfJohns _johnPool;
        public JohnDecorator JohnDecorator { get; private set; }

        public JohnCollectionGame() {
            OnLoad.Add(LoadGame);
            OnUpdate.Add(CameraTracking);
            OnUnload.Add(UnloadGame);
            PhysicsScale = Constants.PHYSICS_SIM_SCALE;
            _johnPool = new PoolOfJohns(this);
        }

        private void CameraTracking() {
            Camera.Position = grabbyClaw.Position;
        }

        private void LoadGame() {
            Camera.TileInputSize = Constants.INPUT_TILE_SIZE;
            Camera.Scale = Constants.CAMERA_SCALE;

            OnWriteDebug.Add(DrawDebugText);

            TileMapTexture = Content.Load<Texture2D>(Constants.TILEMAP_TEXTURE);
            JohnDecorator = new JohnDecorator(GraphicsDevice,TileMapTexture,Constants.JOHN_ANIMATION_SOURCE);
            TileMap = TileMap.CreateFromJSON(Constants.MAP_FILE);

            GenerateWorldCollision(tileValue => !Constants.NON_COLLIDING_TILES.Contains(tileValue));

            PhysicsWorld.Gravity = new Vector2(0,Constants.GRAVITY);

            SetCameraBounds();
            Camera.MinX += Constants.CAM_OFFSET_MIN_X;
            Camera.MaxX += Constants.CAM_OFFSET_MAX_X;
            Camera.MinY += Constants.CAM_OFFSET_MIN_Y;
            Camera.MaxY += Constants.CAM_OFFSET_MAX_Y;

            GenerateGenericTileDictionary();

            grabbyClaw = new GrabbyClaw() {
                MinX = Constants.CLAW_OFFSET_MIN_X,
                MaxX = TileMap.Width + Constants.CLAW_OFFSET_MIN_X,
                MinY = Constants.CLAW_OFFSET_MIN_Y,
                MaxY = TileMap.Height - Constants.CLAW_OFFSET_MAX_Y,
                Position = Constants.GRABBY_CLAW_START_POS
            };

            Entities.Add(grabbyClaw);
        }

        private void UnloadGame() {
            JohnDecorator?.Unload();
            JohnDecorator = null;
        }

        private void DrawDebugText(DebugWriter writer) {
            writer.ToTopLeft();
            writer.Write(Camera.Position,"Camera Position");
            writer.Write(Camera.TileStart,"Tile Start");
            writer.Write(Camera.RenderOrigin,"Render Origin");
        }
    }
}
