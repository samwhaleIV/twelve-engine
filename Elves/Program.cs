﻿using TwelveEngine.Shell;
using Elves.Battle;
using Elves.UI.Font;
using Elves.UI;
using System.Collections.Generic;
using Elves.Battle.Battles;
using Microsoft.Xna.Framework;
using System;
using Elves.SplashMenu;
using Elves.TestStates;
using TwelveEngine;

#pragma warning disable CS0162
#pragma warning disable CS0028

namespace Elves {
    public static class Program {

        public static GameState GetStartState() {
            return new StateTransitionTest();
            return new SongTest();
            return new SplashMenuState();   

            var battle = new BattleSequencer(new DebugBattle(),"Backgrounds/checkerboard");

            battle.OnBattleFinished += battleResult => {
                Console.WriteLine($"Battle result: {Enum.GetName(typeof(BattleResult),battleResult)}");
            };
            return battle;
        }

        private static void SetCustomCursor() {
            var game = Game;
            game.CustomCursorTexture = UITextures.Panel;
            game.CursorSources.Add(CursorState.Default,new Rectangle(64,0,8,8));
            game.CursorSources.Add(CursorState.Interact,new Rectangle(64,8,8,8));
            game.CursorSources.Add(CursorState.Pressed,new Rectangle(72,8,8,8));
            game.CursorScale = 8;
        }
        public static GameManager Game { get; private set; }

        public static void Main(GameManager game) {
            Game = game;

            UITextures.Load(game);
            Fonts.Load();

            if(!Flags.Get(Constants.Flags.OSCursor)) {
                SetCustomCursor();
            }

            game.SetState(GetStartState);
        }
    }
}

#pragma warning restore CS0162
#pragma warning restore CS0028
