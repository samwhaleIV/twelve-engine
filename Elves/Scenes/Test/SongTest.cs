using TwelveEngine.Shell;
using FMOD.Studio;
using TwelveEngine.Audio;
using System.Linq;

namespace Elves.Scenes.Test {
    public sealed class SongTest:InputGameState {

        public SongTest() {
            Name = "Song Test Player";

            OnLoad.Add(Load);
            OnUnload.Add(AudioSystem.Unload);
            OnUpdate.Add(AudioSystem.Update,TwelveEngine.EventPriority.First);

            Impulse.Router.OnAcceptDown += OnAcceptDown;
            Impulse.Router.OnCancelDown += Router_OnCancelDown;
        }

        private BankWrapper musicBank;

        private void Router_OnCancelDown() {
            if(!song.HasValue) {
                return;
            }
            song.Value.FadeOut();
            song = null;
        }

        private EventInstanceController? song;

        private void OnAcceptDown() {
            if(!musicBank.HasEvents || song.HasValue) {
                return;
            }
            song = musicBank.Events["beach"].Create().SetVolume(0.2f).Play();
        }

        private void Load() {
            AudioSystem.Load();
            var strings = AudioSystem.LoadBank("Content/Music/Master.strings.bank");
            var master = AudioSystem.LoadBank("Content/Music/Master.bank");
            musicBank = AudioSystem.LoadBank("Content/Music/Music.bank");

            strings.LogEvents();
            master.LogEvents();
            musicBank.LogEvents();
        }
    }
}
