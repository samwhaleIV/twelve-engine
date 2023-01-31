using Microsoft.Xna.Framework.Media;
using TwelveEngine.Shell;

namespace Elves.Scenes.Test {
    public sealed class SongTest:GameState {
        public SongTest() {
            Name = "Song Test Player";
            OnLoad += SongTest_OnLoad;
        }

        private void SongTest_OnLoad() {
            var song = Content.Load<Song>(Constants.Songs.UV2T3);
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 1f;       
        }
    }
}
