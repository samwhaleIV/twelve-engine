using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Game2D.Entities;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Game2D {
    public sealed class TestGame2D:GameState2D {

        private static class Constants {
            public const float INPUT_TILE_SIZE = 16f;
            public const float CAMERA_SCALE = 10f;

            public const float GRAVITY = 200f;

            public const float SURFACE_FRICTION = 1f;
            public const float SURFACE_MASS = 10f;

            public const float PLAYER_FRICTION = 0f;
            public const float PLAYER_LINEAR_DAMPING = 8f;
            public const float PLAYER_MASS = 1f;
            public const float PLAYER_RESTITUTION = 0f;
            public const float PLAYER_IMPULSE_FORCE = 4f;

            public const string TILEMAP_TEXTURE = "spritesheet";
            public const string MAP_CSV_FILE = "testmap1.csv";

            public const float PHYSICS_SIM_SCALE = 8f;
        }

        public TestGame2D() {
            OnLoad.Add(LoadGame);
            OnUpdate.Add(FollowPlayerWithCamera);
            PhysicsScale = Constants.PHYSICS_SIM_SCALE;
        }

        private Player _player;


        private void LoadGame() {
            Camera.TileInputSize = Constants.INPUT_TILE_SIZE;
            Camera.Scale = Constants.CAMERA_SCALE;

            OnWriteDebug.Add(DrawPlayerPosition);

            TileMapTexture = Content.Load<Texture2D>(Constants.TILEMAP_TEXTURE);
            ImportTiledCSV(Constants.MAP_CSV_FILE,new HashSet<ushort> { 1 },setCameraBounds: true,surfaceFriction: Constants.SURFACE_FRICTION,surfaceMass: Constants.SURFACE_MASS);
            TileDictionary.Add(1,new TileData() {
                Color = Color.White,
                Source = new Rectangle(16,0,16,16)
            });
            TileDictionary.Add(2,new TileData() {
                Color = Color.White,
                Source = new Rectangle(32,0,16,16)
            });

            _player = new Player(TileMapTexture,new TileData() {
                Color = Color.White,
                Source = new Rectangle(80,0,16,16)
            }) {
                Friction = Constants.PLAYER_FRICTION,
                Restitution = Constants.PLAYER_RESTITUTION,
                LinearDamping = Constants.PLAYER_LINEAR_DAMPING,
                Mass = Constants.PLAYER_MASS,
                ImpulseForce = Constants.PLAYER_IMPULSE_FORCE
            };
            Entities.Add(_player);
            _player.Position = new Vector2(5,17);
            PhysicsWorld.Gravity = new Vector2(0,Constants.GRAVITY);
        }

        private void FollowPlayerWithCamera() {
            Camera.Position = _player.Position;
        }

        private void DrawPlayerPosition(DebugWriter writer) {
            writer.ToTopLeft();
            writer.Write(_player.Position,"Player Position");
            writer.Write(Camera.GetRenderLocation(_player),"Player Render Position");
            writer.Write(Camera.RenderOrigin,"Tile Origin");
        }
    }
}
