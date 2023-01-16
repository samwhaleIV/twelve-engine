using Elves.UI;
using System;
using System.Text;
using System.Threading.Tasks;
using TwelveEngine;
using Elves.UI.Battle;
using Elves.Battle.Sprite;
using Microsoft.Xna.Framework.Graphics;

namespace Elves.Battle {
    public class BattleSequencer:BattleRendererState {
         
        private readonly Script _script;
        public Script Script => _script;

        public Action<BattleResult> OnBattleFinished;

        public BattleSequencer(Script script,string background) : base(background) {
            _script = script;
            _script.SetSequencer(this);
            OnLoad += BattleSequencer_OnLoad;
        }

        public BattleSequencer(Script script,Texture2D background) : base(background) {
            _script = script;
            _script.SetSequencer(this);
            OnLoad += BattleSequencer_OnLoad;
        }

        public BattleSequencer(Script script) : base() {
            _script = script;
            _script.SetSequencer(this);
            OnLoad += BattleSequencer_OnLoad;
        }

        private async void BattleSequencer_OnLoad() {
            var battleResult = await _script.Main();
            OnBattleFinished?.Invoke(battleResult);
        }

        private TaskCompletionSource<int> buttonTask;

        private static readonly string[] DefaultOptions = new string[] { Constants.Battle.ContinueText };

        public Task ContinueButton() => GetButton(true,DefaultOptions);

        public Task<int> GetButton(bool isContinue,params string[] options) {
            if(options.Length < 1) {
                options = DefaultOptions;
            }
            int end = Math.Min(options.Length,4);
            for(int i = 0;i<end;i++) {
                ActionButton button = UI.GetActionButton(i);
                StringBuilder buttonLabel = button.Label;
                buttonLabel.Clear();
                buttonLabel.Append(options[i]);
            }
            
            switch(options.Length) {
                case 1:
                    ConfigSingleButton(isContinue);
                    break;
                case 2:
                    ConfigDoubleButtons();
                    break;
                case 3:
                    ConfigTripleButtons();
                    break;
                case 4:
                    ConfigQuadButtons();
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
            battleSprite?.SetPosition(SpritePosition.Left);
            var stringBuilder = UI.SpeechBox.Text;
            stringBuilder.Clear();
            stringBuilder.Append(text);
        }

        public void HideSpeech(BattleSprite battleSprite) {
            battleSprite?.SetPosition(SpritePosition.Center);
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

        protected override void UpdateGame() {
            UpdateUI();        // sync
            UpdateInputs();    // sync
            base.UpdateGame(); // sync
            //do more advanced/long-running async validations here...
        }

        protected override UserData GetPlayerData() => Script.Player;
        protected override UserData GetTargetData() => Script.Actor;

        private void ConfigSingleButton(bool isContinue) {
            UI.ActionButton1.SetState(Now,new ButtonState(isContinue ? ButtonPosition.CenterBottom : ButtonPosition.CenterMiddle,true));
            UI.ActionButton2.Hide(Now); UI.ActionButton3.Hide(Now); UI.ActionButton4.Hide(Now);
        }

        private void ConfigDoubleButtons() {
            UI.ActionButton1.SetState(Now,new ButtonState(ButtonPosition.CenterLeft,true));
            UI.ActionButton2.SetState(Now,new ButtonState(ButtonPosition.CenterRight,true));
            UI.ActionButton3.Hide(Now);
            UI.ActionButton4.Hide(Now);
        }

        private void ConfigTripleButtons() {
            UI.ActionButton1.SetState(Now,new ButtonState(ButtonPosition.TopLeft,true));
            UI.ActionButton2.SetState(Now,new ButtonState(ButtonPosition.TopRight,true));
            UI.ActionButton3.SetState(Now,new ButtonState(ButtonPosition.CenterBottom,true));
            UI.ActionButton4.Hide(Now);
        }

        private void ConfigQuadButtons() {
            UI.ActionButton1.SetState(Now,new ButtonState(ButtonPosition.TopLeft,true));
            UI.ActionButton2.SetState(Now,new ButtonState(ButtonPosition.TopRight,true));
            UI.ActionButton3.SetState(Now,new ButtonState(ButtonPosition.BottomLeft,true));
            UI.ActionButton4.SetState(Now,new ButtonState(ButtonPosition.BottomRight,true));
        }
    }
}
