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
            public GameState2DTest() {
                OnLoad.Add(LoadSpriteSheet,EventPriority.First);
                OnLoad.Add(CreateEntities);
            }
            private void LoadSpriteSheet() {
                SpriteSheet = Content.Load<Texture2D>("spritesheet");
            }
            private void CreateEntities() {
                Entities.Add(new TestSquare(new Rectangle(0,0,64,64),new Vector2(16),Color.White));
                Entities.Add(new TestSquare(new Rectangle(4,0,4,4),new Vector2(1,1),Color.Red));
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
