using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D.Entities {
    public sealed class PlayerDebugger:Entity, IUpdateable {
        public override void Load() {}
        public override void Unload() {}

        private readonly Player player;
        private GameManager game;

        public PlayerDebugger(Player player) {
            this.player = player;
            game = player.Grid.Game;
        }

        private bool skippedFrames = false;
        public void Update(GameTime gameTime) {
            var x = this.player.X;


            if(game.PlaybackFile == "playback/wall-glitch.input" && game.Frame == 1287) {
                game.Pause();
            }

            if(!skippedFrames && game.PlaybackFile == "playback/hell-glitch.input") {
                skippedFrames = true;
                return;
                game.Paused = true;
                game.SkipFrames(760);
            }     

        }
    }
}
