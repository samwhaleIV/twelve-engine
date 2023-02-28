using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwelveEngine;
using Elves.Animation;
using Elves.ElfData;
using TwelveEngine.Effects;
using Microsoft.Xna.Framework;

namespace Elves.Battle.Scripting {
    public abstract partial class BattleScript {

        public virtual void Setup() {
            CreatePlayer();
            CreateActor(ElfManifest.Get(ElfSource.ID));
        }

        public void TODO() {
            throw new NotImplementedException();
        }

        public abstract Task<BattleResult> Main();

        /// <remarks>DEPENDENCY INJECTION, WOOOOOOO!</remarks>
        internal void SetSequencer(BattleSequencer sequencer) => _sequencer = sequencer;

        protected const int B1 = 0, B2 = 1, B3 = 2, B4 = 3;

        /// <summary>
        /// For debug and logging purposes.
        /// </summary>
        public Elf ElfSource { get; set; }
        public Random Random { get; private init; }

        private BattleSequencer _sequencer;
        private BattleSprite _actorSprite;
        private UserData _playerData, _actorData;

        public bool EverybodyIsAlive => Player.IsAlive && Actor.IsAlive;
        public bool EveryBodyIsDead => Player.IsDead && Actor.IsDead;

        public UserData Player => GetPlayerData();
        public UserData Actor => GetActorData();

        public BattleSprite ActorSprite { get => GetActorSprite(); set => SetActor(value); }

        protected ScriptThreader Threader { get; private init; }

        public BattleScript() {
            Threader = new(this);
            Random = Flags.Get(Constants.Flags.FixedBattleRandom) ? new Random(Constants.Battle.FixedSeed) : new Random();
        }

        public async Task Continue() => await _sequencer.ContinueButton();

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

        public async Task<TResult> ShowMiniGame<TResult>(ResultMiniGame<TResult> miniGame) {
            _sequencer.ShowMiniGame(miniGame);
            var result = await miniGame.GetResult();
            _sequencer.HideMiniGame();
            return result;
        }

        public async Task<int> GetButton(LowMemoryList<string> options) {
            return await _sequencer.GetButton(false,options);
        }

        public async Task Tag(LowMemoryList<string> tags) {
            foreach(var tag in tags) {
                _sequencer.SetTag(tag);
                await Continue();
            }
            _sequencer.HideTag();
        }

        public async Task Speech(LowMemoryList<string> speeches) {
            foreach(var speech in speeches) {
                _sequencer.ShowSpeech(speech,ActorSprite);
                await Continue();
            }
            _sequencer.HideSpeech(ActorSprite);
        }
    }
}
