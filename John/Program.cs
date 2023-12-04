﻿using System;
using System.IO;
using TwelveEngine.Shell;
using TwelveEngine;
using TwelveEngine.Game2D;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game2D.Entities;
using Microsoft.Xna.Framework;

namespace John {
    public sealed class Program:EntryPoint {

        [STAThread]
        public static void Main(string[] args) {
            var program = new Program();
            program.StartEngine(args);
        }

        private static GameState GetStartState() {
            return new CollectionGame();
        }

        private void StartEngine(string[] args) {
            string saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"JohnCollectionGame");
            Config.SetBool(Config.Keys.LimitFrameDelta,true);
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
