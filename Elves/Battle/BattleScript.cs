using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TwelveEngine;
using Elves.Animation;
using Elves.ElfData;

namespace Elves.Battle {
    public abstract class BattleScript {
        public abstract Task<BattleResult> Main();

        /// <summary>
        /// For debug and logging purposes.
        /// </summary>
        public Elf ElfSource { get; set; }

        private BattleSequencer _sequencer;

        private readonly Random _random = Flags.Get(Constants.Flags.FixedBattleRandom) ? new Random(Constants.Battle.FixedSeed) : new Random();
        private BattleSprite _actorSprite;

        private UserData _playerData, _actorData;

        private readonly Dictionary<int,int> _threadIndicies = new();

        protected int LastThreadIndex { get; private set; }
        protected int LastThreadSize { get; private set; }
        protected bool LastThreadAtEnd { get; private set; }

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

        public UserData Player {
            get {
                var data = _playerData;
                if(data == null) {
                    Logger.WriteLine("Player UserData is null, creating a generic one.",LoggerLabel.Script);
                    data = CreateFallbackPlayerData();
                    _playerData = data;
                }
                return data;
            }
        }

        public UserData Actor {
            get {
                var data = _actorData;
                if(data == null) {
                    Logger.WriteLine("Actor UserData is null, creating a generic one.",LoggerLabel.Script);
                    data = GetFallbackUserData();
                }
                return data;
            }
        }

        protected void CreatePlayer(string name = null) {
            _playerData = new(string.IsNullOrEmpty(name) ? Constants.Battle.PlayerName : name);
        }

        protected void CreateActor(BattleSprite sprite) {
            _sequencer.Entities.Add(sprite);
            SetActor(sprite);
        }

        protected void CreateActor(Elf elf) {
            BattleSprite sprite = new(elf);
            _sequencer.Entities.Add(sprite);
            SetActor(sprite);
        }

        private void SetActor(BattleSprite sprite) {
            _sequencer.Background.Color = sprite.UserData.Color;
            _actorData = sprite.UserData;
            _actorSprite = sprite;
        }

        private static UserData GetFallbackUserData() {
            return new(Constants.Battle.NoName);
        }

        private static UserData CreateFallbackPlayerData() {
            return new(Constants.Battle.PlayerName);
        }

        private static readonly Dictionary<AnimationType,FrameSet> FallbackFrameSets = new() {
            { AnimationType.Static, AnimationFactory.CreateStatic(0,0,4,8) }
        };

        private static BattleSprite GetFallbackSprite() {
            return new BattleSprite(GetFallbackUserData(),Program.Textures.Missing,8,FallbackFrameSets);
        }

        public BattleSprite ActorSprite {
            get {
                if(_actorSprite == null) {
                    CreateActor(GetFallbackSprite());
                }
                return _actorSprite;
            }
            set => SetActor(value);
        }

        protected async Task Tag(string tag) {
            _sequencer.ShowTag(tag);
            await _sequencer.ContinueButton();
            _sequencer.HideTag();
        }

        protected async Task Speech(string speech) {
            _sequencer.ShowSpeech(speech,ActorSprite);
            await _sequencer.ContinueButton();
            _sequencer.HideSpeech(ActorSprite);
        }

        protected async Task Tag(params string[] tags) {
            foreach(var tag in tags) {
                _sequencer.ShowTag(tag);
                await _sequencer.ContinueButton();
            }
            _sequencer.HideTag();
        }

        protected async Task Speech(params string[] speeches) {
            foreach(var speech in speeches) {
                _sequencer.ShowSpeech(speech,ActorSprite);
                await _sequencer.ContinueButton();
            }
            _sequencer.HideSpeech(ActorSprite);
        }

        protected int GetThreadIndex(int threadID) {
            _threadIndicies.TryGetValue(threadID,out int index);
            return index;
        }

        private void SetThreadIndex(int threadID,int index) {
            _threadIndicies[threadID] = index;
        }

        private string GetThreadValue(int threadID,ThreadMode threadMode,string[] values) {
            int index = GetThreadIndex(threadID);

            LastThreadIndex = index;
            LastThreadAtEnd = index >= values.Length - 1;
            LastThreadSize = values.Length;

            if(threadMode.HasFlag(ThreadMode.NoRepeat) && index >= values.Length) {
                return null;
            }

            string value = values[index];
            if(threadMode.HasFlag(ThreadMode.Random)) {
                index = _random.Next(0,values.Length);
                SetThreadIndex(threadID,index);
                return value;
            }

            index += 1;
            if(index < values.Length) {
                SetThreadIndex(threadID,index);
                return value;
            }

            if(threadMode.HasFlag(ThreadMode.NoRepeat)) {
                index = values.Length;
            } else if(threadMode.HasFlag(ThreadMode.RepeatLast) && values.Length >= 1) {
                index = values.Length - 1;
            } else if(threadMode.HasFlag(ThreadMode.SkipFirstOnRepeat) && values.Length > 1) {
                index = 1;
            } else {
                index = 0;
            }

            SetThreadIndex(threadID,index);
            return value;
        }

        protected async Task TagThread(ThreadMode threadMode,string[] tags,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,tags);
            if(threadValue == null) {
                return;
            }
            _sequencer.ShowTag(threadValue);
            await _sequencer.ContinueButton();
            _sequencer.HideTag();
        }

        protected async Task SpeechThread(ThreadMode threadMode,string[] speeches,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,speeches);
            if(threadValue == null) {
                return;
            }
            _sequencer.ShowSpeech(threadValue,ActorSprite);
            await _sequencer.ContinueButton();
            _sequencer.HideSpeech(ActorSprite);
        }

        protected async Task<int> GetButton(params string[] options) {
            return await _sequencer.GetButton(false,options);
        }

        protected async Task Continue() {
            throw new Exception();
            await _sequencer.ContinueButton();
        }

        internal void SetSequencer(BattleSequencer sequencer) {
            _sequencer = sequencer;
        }
    }
}
