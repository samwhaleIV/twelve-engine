using TwelveEngine.Shell;
using Elves.Scenes.Carousel.UI;

namespace Elves.Scenes.Carousel {
    public sealed class CarouselMenu:CarouselScene3D {

        private readonly CarouselUI UI;

        public CarouselMenu(bool animateLastBattleProgress) : base(animateLastBattleProgress) {
            Name = "Carousel Menu";
            UI = new(this);
            UI.BindInputEvents(this);
            OnPreRender.Add(PreRenderCarouselScene);
            OnRender.Add(Render);
            OnUpdate.Add(Update);
            OnLoad.Add(LoadUI);
            OnInputActivated += UI.FocusDefault;
            OnUnload.Add(SaveSettingsVolume);
        }


        private void SaveSettingsVolume() {
            Program.GlobalSave.SetValue(SaveKeys.MusicVolume,UI.SettingsPage.MusicVolume);
            Program.GlobalSave.SetValue(SaveKeys.SoundVolume,UI.SettingsPage.SoundVolume);
            Program.GlobalSave.TrySave();
        }

        private void LoadUI() {
            UI.SetFirstPage(UI.DefaultPage);
        }

        private void Update() {
            UI.Update();
            CustomCursor.State = UI.CursorState;
            UpdateCarouselItems();
        }

        private void Render() {
            UI.Render(SpriteBatch);
        }
    }
}
