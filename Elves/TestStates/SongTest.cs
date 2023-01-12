using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Shell;

namespace Elves.TestStates {
    public sealed class SongTest:GameState {
        public SongTest() {
            Name = "Song Test Player";
            OnLoad += SongTest_OnLoad;
        }

        private void SongTest_OnLoad() {
            var song = Game.Content.Load<Song>("Music/UV2T3");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 1f;
            
        }
    }
}
