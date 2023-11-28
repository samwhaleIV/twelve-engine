using System;
using System.IO;
using TwelveEngine.Shell;
using TwelveEngine;
using TwelveEngine.Game2D;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game2D.Entities;
using Microsoft.Xna.Framework;

namespace TestPlatform {
    public sealed class Program:EntryPoint {

        [STAThread]
        public static void Main(string[] args) {
            var program = new Program();
            program.StartEngine(args);
        }

        private sealed class GameState2DTest:GameState2D {

            private Texture2D texture;

            public GameState2DTest() {
                OnLoad.Add(LoadSpriteSheet,EventPriority.First);
                OnLoad.Add(CreateEntities);

                int width = 100;
                int height = 100;

                ushort brick = 1;

                ushort[] tileMapData = new ushort[width * height];

                for(int i = 0;i<tileMapData.Length;i++) {
                    tileMapData[i] = brick;
                }

                tileMapData[0] = 2;

                Camera.TileInputSize = 16f;
                Camera.Scale = 4;

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

                TileMap = new TileMap() { Width = width,Height = height,Data = tileMapData };

                Camera.MinX = 0;
                Camera.MinY = 0;
            }

            private void CreateEntities() { 
                //Entities.Add(new TestSquare(texture,new Rectangle(16,16,16,16),new Vector2(1,1),Color.White));
            }

            private void LoadSpriteSheet() {
                texture = Content.Load<Texture2D>("spritesheet");
                TileMapTexture = texture;
            }
        }

        private static GameState GetStartState() {
            return new GameState2DTest();
        }

        private void StartEngine(string[] args) {
            string saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"TwelveEngineTest");
            EngineMain(saveDirectory,args);
        }

        protected override void OnGameLoad(GameStateManager game,string saveDirectory) {
            //TODO: load save data
            //TODO: load audio banks
            game.SetState(GetStartState());
        }

        protected override void OnGameCrashed() {
            //TODO: Save data
        }
    }
}
