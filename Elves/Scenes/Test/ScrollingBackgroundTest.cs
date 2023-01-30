﻿using System;
using TwelveEngine;
using TwelveEngine.Shell;

namespace Elves.Scenes.Test {
    public sealed class ScrollingBackgroundTest:GameState {

        private readonly InfinityBackground infinityBackground = new() {
            Texture = Program.Textures.GiftPattern
        };

        public ScrollingBackgroundTest() {
            OnRender += ScrollingBackgroundTest_OnRender;
            OnLoad += ScrollingBackgroundTest_OnLoad;
        }

        private void ScrollingBackgroundTest_OnLoad() {
            infinityBackground.Load(Game.Content);
        }

        private void ScrollingBackgroundTest_OnRender() {
            infinityBackground.Rotation = (float)(Now / TimeSpan.FromSeconds(4)) * 360;
            infinityBackground.Render(Game.SpriteBatch,Game.Viewport);
        }
    }
}
