using TwelveEngine.Shell;
using FMOD.Studio;
using TwelveEngine.Audio;
using System.Linq;
using System;
using TwelveEngine.Effects;
using Microsoft.Xna.Framework;

namespace Elves.Scenes.Test {
    public sealed class SongTest:InputGameState {

        public SongTest() {
            Name = "Song Test Player";

            OnLoad.Add(Load);

            ImpulseHandler.Router.OnDirectionDown += Router_OnDirectionDown;
        }

        private void Router_OnDirectionDown(TwelveEngine.Direction direction) {
            switch(direction) {
                case TwelveEngine.Direction.Left:
                    song.SetParameter("BaseTrack",1);
                    song.SetParameter("ATrack",0);
                    song.SetParameter("BTrack",0);

                    break;
                case TwelveEngine.Direction.Up:
                    song.SetParameter("BaseTrack",0);
                    song.SetParameter("ATrack",1);
                    song.SetParameter("BTrack",0);
                    break;
                case TwelveEngine.Direction.Right:
                    song.SetParameter("BaseTrack",0);
                    song.SetParameter("ATrack",0);
                    song.SetParameter("BTrack",1);
                    break;
                default:
                    song.FadeOut();
                    break;
            }
        }

        private ManagedEventInstance song = null;

        private void Load() {
            song = Program.AudioBank.Events["menu"].Create();

            song.SetParameter("BaseTrack",1);
            song.SetParameter("ATrack",0);
            song.SetParameter("BTrack",0);

            song = song.SetVolume(0.3f).Play();
        }
    }
}
