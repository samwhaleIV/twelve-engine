using System;
using System.Threading.Tasks;
using TwelveEngine;
using Microsoft.Xna.Framework.Graphics;
using Elves.Scenes.Battle.UI;
using Elves.Scenes.Battle;
using Elves.Scenes;

namespace Elves.Battle {
    public class BattleSequencer:BattleRendererScene {
         
        public BattleScript Script { get; private set; }

        private readonly struct ScriptCompletionData {
            public readonly BattleResult BattleResult { get; init;  }
            public readonly Exception Exception { get; init; }
        } 

        private ScriptCompletionData? _scriptCompletionData = null;

        private void Initialize(BattleScript script) {
            Name = $"{script.ElfSource.Name} battle (ID: {(int)script.ElfSource.ID})";
            script.SetSequencer(this);
            Script = script;
            OnLoad += RunScript;
            OnUpdate += CheckForScriptEnd;
        }

        private void CheckForScriptEnd() {
            if(!_scriptCompletionData.HasValue) {
                return;
            }
            ScriptCompletionData data = _scriptCompletionData.Value;
            if(data.Exception is not null) {
                throw data.Exception;
            }
            EndScene(ExitValue.Get(data.BattleResult));
        }

        public BattleSequencer(BattleScript script,string background):base(background) => Initialize(script);
        public BattleSequencer(BattleScript script,Texture2D background):base(background) => Initialize(script);
        public BattleSequencer(BattleScript script):base() => Initialize(script);

        private async void RunScript() {
            try {
                BattleResult battleResult = await Script.Main();
                _scriptCompletionData = new() {
                    BattleResult = battleResult,
                    Exception = null
                };
            } catch(Exception expcetion) {
                _scriptCompletionData = new() {
                    BattleResult = BattleResult.Stalemate,
                    Exception = expcetion
                };
            }
        }

        private TaskCompletionSource<int> buttonTask;

        private static readonly string[] DefaultOptions = new string[] { Constants.Battle.ContinueText };

        public Task ContinueButton() => GetButton(true,DefaultOptions);

        public Task<int> GetButton(bool isContinue,params string[] options) {
            if(options.Length < 1) {
                options = DefaultOptions;
            }

            for(int i = 0;i<4;i++) {
                var button = UI.GetActionButton(i);
                button.ClearKeyFocus();
                button.CanInteract = false;
            }

            int end = Math.Min(options.Length,4);
            for(int i = 0;i<end;i++) {
                var button = UI.GetActionButton(i);
                button.CanInteract = true;
                var buttonLabel = button.Label;
                buttonLabel.Clear();
                buttonLabel.Append(options[i]);
            }

            UI.ResetInteractionState();
            switch(options.Length) {
                case 1: ConfigSingleButton(isContinue); break;
                case 2: ConfigDoubleButtons(); break;
                case 3: ConfigTripleButtons(); break;
                case 4: ConfigQuadButtons(); break;
            }
            UI.FocusDefault();

            if(buttonTask != null) {
                buttonTask.SetResult(-1);
                Logger.WriteLine("Fatal script error! Tried to get more than one button at a time...",LoggerLabel.Script);
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

            UI.DefaultFocusElement = UI.Button1;
        }

        private void ConfigDoubleButtons() {
            UI.Button1.SetState(Now,new(ButtonPosition.CenterLeft,true));
            UI.Button2.SetState(Now,new(ButtonPosition.CenterRight,true));
            UI.Button3.Hide(Now);
            UI.Button4.Hide(Now);

            UI.DefaultFocusElement = UI.Button1;

            UI.Button1.FocusSet = new() { Right = UI.Button2 };
            UI.Button2.FocusSet = new() { Left = UI.Button1 };
        }

        private void ConfigTripleButtons() {
            UI.Button1.SetState(Now,new(ButtonPosition.TopLeft,true));
            UI.Button2.SetState(Now,new(ButtonPosition.TopRight,true));
            UI.Button3.SetState(Now,new(ButtonPosition.CenterBottom,true));
            UI.Button4.Hide(Now);

            UI.DefaultFocusElement = UI.Button1;

            UI.Button1.FocusSet = new() { Right = UI.Button2, Down = UI.Button3 };
            UI.Button2.FocusSet = new() { Left = UI.Button1, Down = UI.Button3 };
            UI.Button3.FocusSet = new() { Up = UI.Button1, Left = UI.Button1, Right = UI.Button2, IndeterminateUp = true };
        }

        private void ConfigQuadButtons() {
            UI.Button1.SetState(Now,new(ButtonPosition.TopLeft,true));
            UI.Button2.SetState(Now,new(ButtonPosition.TopRight,true));
            UI.Button3.SetState(Now,new(ButtonPosition.BottomLeft,true));
            UI.Button4.SetState(Now,new(ButtonPosition.BottomRight,true));

            UI.DefaultFocusElement = UI.Button1;

            UI.Button1.FocusSet = new() { Right = UI.Button2, Down = UI.Button3 };
            UI.Button2.FocusSet = new() { Left = UI.Button1, Down = UI.Button4 };
            UI.Button3.FocusSet = new() { Up = UI.Button1, Right = UI.Button4 };
            UI.Button4.FocusSet = new() { Up = UI.Button2, Left = UI.Button3 };
        }
    }
}
