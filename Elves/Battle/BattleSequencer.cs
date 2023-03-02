using System;
using System.Threading.Tasks;
using TwelveEngine;
using Elves.Scenes.Battle.UI;
using Elves.Scenes.Battle;
using TwelveEngine.Shell;
using Elves.ElfData;
using Elves.Battle.Scripting;

namespace Elves.Battle {
    public class BattleSequencer:BattleRendererScene {

        private const int BUTTON_COUNT = 4;
         
        public BattleScript Script { get; private set; }

        public event Action<GameState,BattleResult,ElfID> OnBattleEnd;

        private void Initialize(BattleScript script) {
            ElfID elfID = script.ElfSource.ID;
            string elfName = script.ElfSource.Name;
            Name = $"{elfName} battle (ID: {(int)elfID})";
            script.SetSequencer(this);
            Script = script;
            transitionTask = new TaskCompletionSource();
            OnLoad.Add(LoadSequencer);
            if(!Flags.Get(Constants.Flags.QuickBattle)) {
                return;
            }
            Impulse.Router.OnDebugDown += () => {
                Logger.WriteLine($"Restarting battle state for battle {elfID} ({elfName}).",LoggerLabel.Debug);
                TransitionOut(new() { State = ElfGame.GetBattleScene(elfID),Duration = TimeSpan.Zero, Data = new() {
                    Flags = StateFlags.CarryInput
                } });
            };
        }

        private void LoadSequencer() {
            AddScriptToScheduler();
            SetupDefaultUIState();
            if(FadeInIsFlagged) {
                OnTransitionIn.Add(StateTransitionIn);
                return;
            }
            StateTransitionIn();
        }

        private void SetupDefaultUIState() {
            TimeSpan now = Now;
            UI.Button1.SetState(now,ButtonState.CenterBottom);
            for(int i = 0;i<BUTTON_COUNT;i++) {
                var button = UI.GetActionButton(i);
                button.ClearKeyFocus();
                button.CanInteract = false;
                button.Hide(now);
                button.FinishAnimation(now);
            }
        }

        private TaskCompletionSource transitionTask;

        private void StateTransitionIn() {
            if(transitionTask is null) {
                return;
            }
            transitionTask.SetResult();
            transitionTask = null;
        }

        public async Task TransitionIn() {
            if(transitionTask is null) {
                if(!FadeInIsFlagged) {
                    return;
                }
                Logger.WriteLine("Tried to transition in, but the game state is already done transitioning.",LoggerLabel.Script);
                return;
            }
            await transitionTask.Task;
        }

        public BattleSequencer(BattleScript script) : base() {
            Initialize(script);
            Background = script.CreateBackground();
        }

        private void AddScriptToScheduler() => GameLoopSyncContext.RunTask(ExecuteScript);

        private TimeSpan exitStart;

        private int endSceneHandle;

        private BattleResult battleResult;

        private void EndSceneDelay() {
            if(Now - exitStart < Constants.AnimationTiming.BattleEndDelay) {
                return;
            }
            OnUpdate.Remove(endSceneHandle);
            OnBattleEnd?.Invoke(this,battleResult,Script.ElfSource.ID);
        }

        private async Task ExecuteScript() {
            Script.Setup();
            await TransitionIn();
            battleResult = await Script.Main();
            HideAllButtons();
            exitStart = Now;
            endSceneHandle = OnUpdate.Add(EndSceneDelay);
        }

        private TaskCompletionSource<int> buttonTask;

        private static readonly LowMemoryList<string> DefaultOptions = new(Constants.Battle.ContinueText);

        public async Task ContinueButton() => await GetButton(true,DefaultOptions);

        public async Task<int> GetButton(bool isContinue,LowMemoryList<string> options) {
            if(options.Size < 1) {
                /* Might want to handle this better if we ever create a opened battle API */
                options = DefaultOptions;
            }

            for(int i = 0;i<BUTTON_COUNT;i++) {
                var button = UI.GetActionButton(i);
                button.ClearKeyFocus();
                button.CanInteract = false;
            }

            int end = Math.Min(options.Size,BUTTON_COUNT);
            int optionIndex = 0;
            foreach(var option in options) {
                var button = UI.GetActionButton(optionIndex);
                button.CanInteract = true;
                var buttonLabel = button.Label;
                buttonLabel.Clear();
                buttonLabel.Append(option);
                optionIndex++;
                if(optionIndex >= end) {
                    break;
                }
            }

            UI.ResetInteractionState();
            switch(options.Size) {
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
            return await buttonTask.Task;
        }

        public void ShowMiniGame(MiniGame miniGame) {
            HideTag();
            UI.ResetInteractionState();
            HideAllButtons();
            miniGame.UpdateState(this);
            var miniGameScreen = UI.MiniGameScreen;
            miniGameScreen.MiniGame = miniGame;
            miniGameScreen.Show(Now);
        }

        public void HideMiniGame() {
            UI.MiniGameScreen.Hide(Now);
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
            HideTag();
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

        public void SetTag(string text) {
            var tagline = UI.Tagline;
            tagline.UpdateAnimationTime(Now);
            if(tagline.IsShown || (!tagline.IsShown && tagline.AnimationProgress <= 0)) {
                tagline.CycleText(Now);
            }
            tagline.CurrentText.Clear();
            tagline.CurrentText.Append(text);
            tagline.Show(Now);
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

        private void HideAllButtons() {
            for(int i = 0;i<BUTTON_COUNT;i++) {
                var button = UI.GetActionButton(i);
                button.ClearKeyFocus();
                button.CanInteract = false;
                button.Hide(Now);
            }
            UI.DefaultFocusElement = null;
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
