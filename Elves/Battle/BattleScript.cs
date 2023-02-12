using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwelveEngine;
using Elves.Animation;
using Elves.ElfData;
using TwelveEngine.Shell;
using TwelveEngine.Effects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Elves.Battle {
    public abstract class BattleScript {

        public abstract void Setup();
        public abstract Task<BattleResult> Main();

        /// <remarks>DEPENDENCY INJECTION, WOOOOOOO!</remarks>
        internal void SetSequencer(BattleSequencer sequencer) => _sequencer = sequencer;

        protected const int Button1 = 0, Button2 = 1, Button3 = 2, Button4 = 3;

        /// <summary>
        /// For debug and logging purposes.
        /// </summary>
        public Elf ElfSource { get; set; }

        private BattleSequencer _sequencer;

        public Random Random { get; private init; }

        private BattleSprite _actorSprite;

        private UserData _playerData, _actorData;

        protected ScriptThreader Threader { get; private init; }

        public BattleScript() {
            Threader = new(this);
            Random = Flags.Get(Constants.Flags.FixedBattleRandom) ? new Random(Constants.Battle.FixedSeed) : new Random();
        }

        #region SEQUENCER UI BINDINGS
        public void ShowSpeech(string speech) => _sequencer.ShowSpeech(speech,ActorSprite);
        public void HideSpeech() => _sequencer.HideSpeech(ActorSprite);

        public void SetTag(string tag) => _sequencer.SetTag(tag);
        public void HideTag() => _sequencer.HideTag();

        public async Task<int> GetButton(params string[] options) => await _sequencer.GetButton(false,options);
        public async Task Continue() => await _sequencer.ContinueButton();
        #endregion

        public BattleResult WinCondition {
            get {
                bool playerAlive = Player.IsAlive;
                bool actorAlive = Actor.IsAlive;

                if(playerAlive == actorAlive) {
                    return BattleResult.Stalemate;
                }
                if(!Player.IsAlive) {
                    return BattleResult.PlayerLost;
                }
                return BattleResult.PlayerWon;
            }
        }

        public bool EverybodyIsAlive => Player.IsAlive && Actor.IsAlive;
        public bool EveryBodyIsDead => Player.IsDead && Actor.IsDead;

        private UserData GetPlayerData() {
            var data = _playerData;
            if(data == null) {
                Logger.WriteLine("Player UserData is null, creating a generic one.",LoggerLabel.Script);
                data = CreateFallbackPlayerData();
                _playerData = data;
            }
            return data;
        }

        private UserData GetActorData() {
            var data = _actorData;
            if(data == null) {
                Logger.WriteLine("Actor UserData is null, creating a generic one.",LoggerLabel.Script);
                data = GetFallbackUserData();
            }
            return data;
        }

        public UserData Player => GetPlayerData();
        public UserData Actor => GetActorData();

        protected UserData CreatePlayer(string name = null) {
            var data = new UserData(string.IsNullOrEmpty(name) ? Constants.Battle.PlayerName : name);
            _playerData = data;
            return data;
        }

        protected UserData CreateActor(BattleSprite sprite) {
            _sequencer.Entities.Add(sprite);
            return SetActor(sprite);
        }

        protected UserData CreateActor(Elf elf) {
            BattleSprite sprite = new(elf);
            _sequencer.Entities.Add(sprite);
            return SetActor(sprite);
        }

        protected virtual void BindActorAnimations(BattleSprite sprite) {
            sprite.UserData.OnHurt += () => sprite.SetAnimation(AnimationType.Hurt);
            sprite.UserData.OnDied += () => sprite.SetAnimation(AnimationType.Dead);
        }

        private UserData SetActor(BattleSprite sprite) {
            var userData = sprite.UserData;
            _actorData = userData;
            _actorSprite = sprite;
            BindActorAnimations(sprite);
            return userData;
        }

        private static UserData GetFallbackUserData() => new(Constants.Battle.NoName);
        private static UserData CreateFallbackPlayerData() => new(Constants.Battle.PlayerName);

        private static readonly Dictionary<AnimationType,FrameSet> FallbackFrameSets = new() {
            { AnimationType.Static, AnimationFactory.CreateStatic(0,0,4,8) }
        };

        private static BattleSprite GetFallbackSprite() {
            return new BattleSprite(GetFallbackUserData(),Program.Textures.Missing,8,FallbackFrameSets);
        }

        private BattleSprite GetActorSprite() {
            if(_actorSprite == null) {
                CreateActor(GetFallbackSprite());
            }
            return _actorSprite;
        }

        public BattleSprite ActorSprite { get => GetActorSprite(); set => SetActor(value); }

        protected async Task Tag(string tag) {
            SetTag(tag);
            await Continue();
            HideTag();
        }

        protected async Task Speech(string speech) {
            ShowSpeech(speech);
            await Continue();
            HideSpeech();
        }

        protected async Task Tag(params string[] tags) {
            foreach(var tag in tags) {
                SetTag(tag);
                await Continue();
            }
            HideTag();
        }

        protected async Task Speech(params string[] speeches) {
            foreach(var speech in speeches) {
                ShowSpeech(speech);
                await Continue();
            }
            HideSpeech();
        }

        public virtual async Task Exit(BattleResult battleResult) {
            //todo: get randomly generated messages - or whatever
            switch(battleResult) {
                case BattleResult.PlayerWon:
                    SetTag("You survived...");
                    await Continue();
                    SetTag("Your journey continues.");
                    break;
                case BattleResult.PlayerLost:
                    SetTag("You died...");
                    await Continue();
                    SetTag("Better luck next time.");
                    break;
                default:
                    SetTag("Everybody is a loser...");
                    await Continue();
                    SetTag("Especially you.");
                    break;
            }
        }

        public virtual ScrollingBackground CreateBackground() => new() {
            TileScale = 4f,
            Scale = 2f,
            Bulge = -0.75f,
            Color = ElfSource.Color,
            Texture = Program.Textures.Nothing,
            ScrollTime = Constants.AnimationTiming.ScrollingBackgroundDefault,
            ColorA = Color.FromNonPremultiplied(new Vector4(new Vector3(0.41f),1)),
            ColorB = Color.FromNonPremultiplied(new Vector4(new Vector3(0.66f),1))
        };
    }
}
