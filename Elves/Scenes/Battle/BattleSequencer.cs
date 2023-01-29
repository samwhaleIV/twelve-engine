using System;
using System.Text;
using System.Threading.Tasks;
using TwelveEngine;
using Microsoft.Xna.Framework.Graphics;
using Elves.Scenes.Battle.UI;
using Elves.Scenes.Battle.Sprite;

namespace Elves.Scenes.Battle {
    public class BattleSequencer:BattleRendererState {
         
        private readonly BattleScript _script;
        public BattleScript Script => _script;

        public BattleSequencer(BattleScript script,string background) : base(background) {
            _script = script;
            _script.SetSequencer(this);
            OnLoad += BattleSequencer_OnLoad;
        }

        public BattleSequencer(BattleScript script,Texture2D background) : base(background) {
            _script = script;
            _script.SetSequencer(this);
            OnLoad += BattleSequencer_OnLoad;
        }

        public BattleSequencer(BattleScript script) : base() {
            _script = script;
            _script.SetSequencer(this);
            OnLoad += BattleSequencer_OnLoad;
        }

        private async void BattleSequencer_OnLoad() {
            BattleResult battleResult = await _script.Main();
            EndScene(ExitValue.Get(battleResult));
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
                Button button = UI.GetActionButton(i);
                StringBuilder buttonLabel = button.Label;
                buttonLabel.Clear();
                buttonLabel.Append(options[i]);
            }
            
            switch(options.Length) {
                case 1: ConfigSingleButton(isContinue); break;
                case 2: ConfigDoubleButtons(); break;
                case 3: ConfigTripleButtons(); break;
                case 4: ConfigQuadButtons(); break;
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

        protected override UserData GetPlayerData() => Script.Player;
        protected override UserData GetTargetData() => Script.Actor;

        private void ConfigSingleButton(bool isContinue) {
            UI.Button1.SetState(Now,new(isContinue ? ButtonPosition.CenterBottom : ButtonPosition.CenterMiddle,true));
            UI.Button2.Hide(Now); UI.Button3.Hide(Now); UI.Button4.Hide(Now);
        }

        private void ConfigDoubleButtons() {
            UI.Button1.SetState(Now,new(ButtonPosition.CenterLeft,true));
            UI.Button2.SetState(Now,new(ButtonPosition.CenterRight,true));
            UI.Button3.Hide(Now);
            UI.Button4.Hide(Now);
        }

        private void ConfigTripleButtons() {
            UI.Button1.SetState(Now,new(ButtonPosition.TopLeft,true));
            UI.Button2.SetState(Now,new(ButtonPosition.TopRight,true));
            UI.Button3.SetState(Now,new(ButtonPosition.CenterBottom,true));
            UI.Button4.Hide(Now);
        }

        private void ConfigQuadButtons() {
            UI.Button1.SetState(Now,new(ButtonPosition.TopLeft,true));
            UI.Button2.SetState(Now,new(ButtonPosition.TopRight,true));
            UI.Button3.SetState(Now,new(ButtonPosition.BottomLeft,true));
            UI.Button4.SetState(Now,new(ButtonPosition.BottomRight,true));
        }
    }
}
