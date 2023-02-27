using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using TwelveEngine;
using TwelveEngine.Shell;

namespace Elves.Scenes.ModeSelectMenu {

    using static Constants.ModeSelectMenu;

    public sealed class ModeSelectMenuScene:TerminalScene {

        private const int NO_SELECTION = -1;

        private int _commandIndex = NO_SELECTION;
        private readonly bool ClassicModeUnlocked = Program.Save?.HasFlag(SaveKeys.ClassicModeUnlocked) ?? false;
        private readonly Command[] _commands;
        public ModeSelectMenuInteractionAgent InteractionAgent { get; private init; }

        public ModeSelectMenuScene(bool skipIntroAnimation) {
            Name = "Mode Select Menu";
            ClearColor = Color.White;
            _commands = new Command[] {
                new(PlayGameText,ModeSelection.PlayGame),
                new(SaveSelectText,ModeSelection.SaveSelect),
                new(ReplayIntroText,ModeSelection.ReplayIntro),
                new(MusicPlayerText,ModeSelection.MusicPlayer),
                new(ClassicModeUnlocked ? ClassicModeText : RedactedText,ModeSelection.ClassicMode),
                new(CreditsText,ModeSelection.Credits)
            };
            PrintMenuLines();
            InteractionAgent = GetInteractionAgent();
            OnRender.Add(RenderCRTStencil,EventPriority.Fourth);
            OnUpdate.Add(UpdateCustomCursor,EventPriority.Second);
            if(!skipIntroAnimation) {
                OnTextQueueEmptied += ModeSelectMenuScene_OnTextQueueEmptied;
            } else {
                FlushTextQueue();
                OnInputActivated += InteractionAgent.FocusDefault;
            }
        }

        private void ModeSelectMenuScene_OnTextQueueEmptied() {
            InteractionAgent.FocusDefault();
            OnTextQueueEmptied -= ModeSelectMenuScene_OnTextQueueEmptied;
        }

        private void UpdateCustomCursor() {
            CustomCursor.State = InteractionAgent.CursorState;
        }

        private void PrintMenuLines() {
            PrintLine(StartMessage);
            PrintLine();
            foreach(var command in _commands) {
                PrintLine(command.Text);
            }
        }

        private readonly struct Command {

            public readonly string Text;
            public readonly ModeSelection Action;

            public Command(string text,ModeSelection modeSelection) {
                Text = text;
                Action = modeSelection;
            }
        }

        public event Action<GameState,ModeSelection> OnSceneEnd;

        private void SelectionTransition(ModeSelection selection) {
            if(selection == ModeSelection.ClassicMode && !ClassicModeUnlocked) {
                //TODO a sound effect or something?
                return;
            }
            OnSceneEnd?.Invoke(this,selection);
        }

        private FloatRectangle GetCommandLineArea(int commandIndex) {
            if(IsWritingText) {
                return FloatRectangle.Empty;
            }
            return GetLineArea(GetLineIndex(commandIndex));
        }

        private TerminalLineInteractionProxy[] GetTerminalLineInteractionProxies() {
            var lineProxies = new TerminalLineInteractionProxy[_commands.Length];
            TerminalLineInteractionProxy lastElement = null;
            bool isWritingText() => IsWritingText;
            for(int commandIndex = 0;commandIndex < lineProxies.Length;commandIndex++) {
                var command = _commands[commandIndex];
                TerminalLineInteractionProxy lineProxy = new() {
                    CommandIndex = commandIndex,
                    GetArea = GetCommandLineArea,
                    PreviousFocusElement = lastElement,
                    CanInteract = true,
                    Selection = command.Action,
                    GetInteractionPaused = isWritingText
                };
                if(lastElement is not null) {
                    lastElement.NextFocusElement = lineProxy;
                }
                lastElement = lineProxy;
                lineProxy.OnActivated += SelectionTransition;
                lineProxies[commandIndex] = lineProxy;
            }
            return lineProxies;
        }

        private ModeSelectMenuInteractionAgent GetInteractionAgent() {
            var lineProxies = GetTerminalLineInteractionProxies();
            ModeSelectMenuInteractionAgent interactionAgent = new(this,lineProxies);
            interactionAgent.BindInputEvents(this);
            interactionAgent.OnSelectionChanged += OnSelectionChanged;
            interactionAgent.DefaultFocusElement = lineProxies[0];
            return interactionAgent;
        }

        private int GetLineIndex(int commandIndex) {
            return Lines.Count - _commands.Length + commandIndex;
        }

        private StringBuilder GetLine(int commandIndex) {
            return Lines[GetLineIndex(commandIndex)].StringBuilder;
        }

        private void ClearOldCommandSelection(int commandIndex) {
            var oldLine = GetLine(commandIndex);
            var suffixLength = SelectedCommandSuffix.Length;
            oldLine.Remove(oldLine.Length-suffixLength,suffixLength);
            oldLine.Remove(0,SelectedCommandPrefix.Length);
        }

        private void DecorateNewCommandSelection(int commandIndex) {
            var newLine = GetLine(commandIndex);
            newLine.Insert(0,SelectedCommandPrefix);
            newLine.Append(SelectedCommandSuffix);
            _commandIndex = commandIndex;
        }

        private void OnSelectionChanged(TerminalLineInteractionProxy lineProxy) {
            if(_commandIndex > NO_SELECTION) {
                ClearOldCommandSelection(_commandIndex);
            }
            if(lineProxy is null) {
                _commandIndex = NO_SELECTION;
                return;
            }
            DecorateNewCommandSelection(lineProxy.CommandIndex);
        }

        private void RenderCRTStencil() {
            SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            var source = new Rectangle(98,0,96,96);
            var scale = UIScale * Constants.Terminal.CRTScale;
            var size = new Vector2(96,96) * scale;
            FloatRectangle area = new(Viewport.Bounds.Center.ToVector2()-size*0.5f,size);
            area.Y += scale * 9 * 0.5f;
            SpriteBatch.Draw(Program.Textures.CRTStencil,area.ToRectangle(),source,Constants.Terminal.BackgroundColor);
            source.X = 0;
            SpriteBatch.Draw(Program.Textures.CRTStencil,area.ToRectangle(),source,Color.White);
            SpriteBatch.End();
        }
    }
}
