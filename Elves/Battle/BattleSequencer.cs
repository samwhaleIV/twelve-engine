using Elves.UI;
using System;
using System.Text;
using System.Threading.Tasks;
using TwelveEngine;
using Elves.UI.Battle;
using System.Security.Cryptography.X509Certificates;
using Elves.Battle.Sprite;

namespace Elves.Battle {
    public class BattleSequencer:BattleRendererState {
         
        private readonly Script _script;

        public Script Script => _script;

        public BattleSequencer(Script script,string backgroundImage) : base(backgroundImage) {
            _script = script;
            _script.SetSequencer(this);
            OnLoad += BattleSequencer_OnLoad;
        }

        private void BattleSequencer_OnLoad() {
            _script.Main();
        }

        private TaskCompletionSource<int> buttonTask;

        private static readonly string[] DefaultOptions = new string[] { "CONTINUE" };

        public Task ContinueButton() {
            return GetButton(true,DefaultOptions);
        }

        public Task<int> GetButton(bool isContinue,params string[] options) {
            if(options.Length < 1) {
                options = DefaultOptions;
            }
            int end = Math.Min(options.Length,4);
            for(int i = 0;i<end;i++) {
                var button = UI.GetActionButton(i);
                var label = button.Label;
                label.Clear();
                label.Append(options[i]);
            }
            switch(options.Length) {
                case 1:
                    UI.ActionButton1.SetState(Now,new ButtonState(isContinue ? ButtonPosition.CenterBottom : ButtonPosition.CenterMiddle,true));
                    UI.ActionButton2.Hide(Now); UI.ActionButton3.Hide(Now); UI.ActionButton4.Hide(Now);
                    break;
                case 2:
                    UI.ActionButton1.SetState(Now,new ButtonState(ButtonPosition.CenterLeft,true));
                    UI.ActionButton2.SetState(Now,new ButtonState(ButtonPosition.CenterRight,true));
                    UI.ActionButton3.Hide(Now);
                    UI.ActionButton4.Hide(Now);
                    break;
                case 3:
                    UI.ActionButton1.SetState(Now,new ButtonState(ButtonPosition.TopLeft,true));
                    UI.ActionButton2.SetState(Now,new ButtonState(ButtonPosition.TopRight,true));
                    UI.ActionButton3.SetState(Now,new ButtonState(ButtonPosition.CenterBottom,true));
                    UI.ActionButton4.Hide(Now);
                    break;
                case 4:
                    UI.ActionButton1.SetState(Now,new ButtonState(ButtonPosition.TopLeft,true));
                    UI.ActionButton2.SetState(Now,new ButtonState(ButtonPosition.TopRight,true));
                    UI.ActionButton3.SetState(Now,new ButtonState(ButtonPosition.BottomLeft,true));
                    UI.ActionButton4.SetState(Now,new ButtonState(ButtonPosition.BottomRight,true));
                    break;
            }
            if(buttonTask != null) {
                buttonTask.SetResult(-1);
                Logger.WriteLine("Fatal script error! Tried to get more than one button at a time...");
            }
            buttonTask = new TaskCompletionSource<int>();
            return buttonTask.Task;
        }

        protected override void ActionButtonClicked(int ID) {
            if(buttonTask == null) {
                return;
            }
            var taskSource = buttonTask;
            buttonTask = null;
            taskSource?.SetResult(ID);
        }

        public void ShowSpeech(string text,BattleSprite battleSprite) {
            UI.SpeechBox.Show(Now);
            battleSprite?.SetSpritePosition(Now,SpritePosition.Left);
            var stringBuilder = UI.SpeechBox.Text;
            stringBuilder.Clear();
            stringBuilder.Append(text);
        }

        public void HideSpeech(BattleSprite battleSprite) {
            battleSprite?.SetSpritePosition(Now,SpritePosition.Center);
            UI.SpeechBox.Hide(Now);
        }

        public void ShowTag(string text) {
            if(UI.Tagline.IsShown) {
                UI.Tagline.CycleText(Now);
            }
            UI.Tagline.CurrentText.Clear();
            UI.Tagline.CurrentText.Append(text);
            UI.Tagline.Show(Now);
        }


        public void HideTag() {
            UI.Tagline.Hide(Now);
        }

        private void UpdateScript() {

        }

        protected override void UpdateGame() {
            UpdateUI();        // sync
            UpdateInputs();    // sync
            base.UpdateGame(); // sync
            UpdateScript();    // async
        }
    }
}
