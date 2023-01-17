using TwelveEngine.Shell;
using Elves.Scenes.Battle;
using Elves.Scenes.Battle.Battles;
using System;
using TwelveEngine;
using Elves.Scenes.Carousel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#pragma warning disable IDE0079
#pragma warning disable CS0162
#pragma warning disable CS0028

namespace Elves {
    public static class Program {

        public static GameState GetStartState() {
            //return new SnowTest();
            //return new ScrollingBackgroundTest();
            return new CarouselMenu();
            //return new SongTest();
            //return new SplashMenuState();   

            var battle = new BattleSequencer(new DebugBattle());

            battle.OnBattleFinished += battleResult => {
                Console.WriteLine($"Battle result: {Enum.GetName(typeof(BattleResult),battleResult)}");
            };
            return battle;
        }

        private static void AddCursorState(
            CursorState cursorState,Texture2D texture,int? originX = null,int? originY = null
        ) {
            Game.CursorSources.Add(cursorState,MouseCursor.FromTexture2D(texture,originX ?? 0,originY ?? 0));
        }

        private static void AddCustomCursors() {
            AddCursorState(CursorState.Default,Textures.CursorDefault,32,32);
            AddCursorState(CursorState.Interact,Textures.CursorAlt1,32,32);
            AddCursorState(CursorState.Pressed,Textures.CursorAlt2,32,32);
        }

        public static GameManager Game { get; private set; }
        public static SaveData SaveData { get; private set; }

        public static void Main(GameManager game,SaveData saveData) {
            Game = game;
            SaveData = saveData;

            Game.Window.Title = "Elves!";

            Textures.Load(game);

            if(!Flags.Get(Constants.Flags.OSCursor)) {
                AddCustomCursors();
                game.UseCustomCursor = true;
            }

            game.SetState(GetStartState);
        }
    }
}

#pragma warning restore CS0162
#pragma warning restore CS0028
#pragma warning restore IDE0079
