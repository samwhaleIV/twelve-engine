using FMOD.Studio;

namespace TwelveEngine.Audio {
    public static class AudioSystem {

        private const int DEFAULT_SAMPLE_RATE = 48000;
        private const int MAX_STUDIO_CHANNELS = 32;

        private const float FALLBACK_VOLUME = 1;

        private static bool _initialized = false, _disposed = false;

        private static FMOD.Studio.System StudioSystem;
        private static FMOD.System CoreSystem;

        private static int _bankIDCounter = 0;
        private static readonly Dictionary<string,int> BankIDs = new();
        private static readonly Dictionary<int,BankWrapper> Banks = new();

        public static float MusicVolume { get => GetMusicVolume(); set => SetMusicVolume(value); }
        public static float SoundVolume { get => GetSoundVolume(); set => SetSoundVolume(value); }

        public static IEnumerable<BankWrapper> GetBanks() => Banks.Values;

        private static void ValidateInitializationState() {
            if(!_initialized) {
                throw new InvalidOperationException("Audio controller has not been initialized.");
            }
            if(_disposed) {
                throw new InvalidOperationException("Audio controller has already been disposed.");
            }
        }

        private static FMOD.INITFLAGS GetSystemInitFlags() {
            var flags = FMOD.INITFLAGS.NORMAL;
            if(Flags.Get(Constants.Flags.FMODProfile)) {
                flags |= FMOD.INITFLAGS.PROFILE_ENABLE;
            }
            return flags;
        }

        internal static void Load() {
            /* Initialization guard */
            if(_disposed) {
                throw new InvalidOperationException("Audio controller has already been disposed.");
            }
            if(_initialized) {
                throw new InvalidOperationException("Audio controller has already been initialized.");
            }
            _initialized = true;
            /* FMOD System initialization */
            FMOD.Studio.System.create(out StudioSystem);
            StudioSystem.getCoreSystem(out CoreSystem);

            int sampleRate = Config.GetIntNullable(Config.Keys.SampleRate) ?? DEFAULT_SAMPLE_RATE;
            CoreSystem.setSoftwareFormat(sampleRate,FMOD.SPEAKERMODE.STEREO,2);

            StudioSystem.initialize(MAX_STUDIO_CHANNELS,INITFLAGS.SYNCHRONOUS_UPDATE,GetSystemInitFlags(),IntPtr.Zero);
        }

        internal static void Update() {
            ValidateInitializationState();
            StudioSystem.update();
        }

        private static int GetBankID(string bankFile) {
            if(BankIDs.ContainsKey(bankFile)) {
                return BankIDs[bankFile];
            }
            int ID = _bankIDCounter;
            _bankIDCounter += 1;
            BankIDs.Add(bankFile,ID);
            return ID;
        }

        public static bool TryLoadBank(string bankFile,out BankWrapper bankWrapper) {
            ValidateInitializationState();
            int bankID = GetBankID(bankFile);
            if(Banks.ContainsKey(bankID)) {
                throw new InvalidOperationException($"Bank '{bankFile}' (ID: {bankID}) has already been loaded.");
            }
            /* 'LOAD_BANK_FLAGS.NORMAL' is blocking. This way, we cannot return a bank that has not been fully loaded */
            var result = StudioSystem.loadBankFile(bankFile,LOAD_BANK_FLAGS.NORMAL,out Bank bank);
            if(result != FMOD.RESULT.OK) {
                Logger.WriteLine($"Could not load '{bankFile}'. Does the file exist?",LoggerLabel.Audio);
                bankWrapper = default;
                return false;
            }
            bankWrapper = new(bank,bankID,bankFile);
            Banks.Add(bankID,bankWrapper);
            bankWrapper.LogEvents();
            return true;
        }

        private static VCA? _musicVCA, _soundVCA;

        private static bool TryGetVCA(string name,out VCA vca) {
            return StudioSystem.getVCA($"vca:/{name}",out vca) == FMOD.RESULT.OK;
        }

        public static void BindVCAs(string musicVCAName = Constants.MusicVCAName,string soundVCAName = Constants.SoundVCAName) {
            ValidateInitializationState();
            if(!_musicVCA.HasValue && TryGetVCA(musicVCAName,out VCA musicVSA)) {
                _musicVCA = musicVSA;
            } else {
                Logger.WriteLine($"Music VCA '{musicVCAName}' not found.",LoggerLabel.Audio);
            }
            if(!_soundVCA.HasValue && TryGetVCA(soundVCAName,out VCA soundVCA)) {
                _soundVCA = soundVCA;
            } else {
                Logger.WriteLine($"Sound VCA '{soundVCAName}' not found.",LoggerLabel.Audio);
            }
        }

        public static bool HasBank(int bankID) {
            ValidateInitializationState();
            return Banks.ContainsKey(bankID);
        }

        public static void UnloadBank(int bankID) {
            ValidateInitializationState();
            if(!Banks.ContainsKey(bankID)) {
                throw new InvalidOperationException($"Bank '{bankID}' does not exist or has already been unloaded.");
            }
            Bank bank = Banks[bankID].Bank;
            Banks.Remove(bankID);
            bank.unload();
        }

        public static bool TryGetBank(int bankID,out BankWrapper bank) {
            ValidateInitializationState();
            return Banks.TryGetValue(bankID,out bank);
        }

        /// <summary>
        /// Cleanup the underlying FMOD audio system. Must been called before the game terminates.<br/>
        /// I honestly don't know what happens if you don't, but it probably doesn't make the operating system very happy.<br/><br/>
        /// Cleanup includes previously loaded audio banks contained in <see cref="Banks"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        internal static void Unload() {
            ValidateInitializationState();
            _disposed = true;
            foreach(var bank in Banks.Values) {
                bank.Bank.unload();
            }
            Banks.Clear();
            StudioSystem.release();
            /* The core audio system is automatically released with the studio system. */;
        }

        private static bool TryGetMusicVCA(out VCA vca) {
            vca = _musicVCA ?? default;
            return _musicVCA.HasValue;
        }

        private static bool TryGetSoundVCA(out VCA vca) {
            vca = _soundVCA ?? default;
            return _soundVCA.HasValue;
        }

        private static float GetMusicVolume() {
            ValidateInitializationState();
            float volume = FALLBACK_VOLUME;
            if(TryGetMusicVCA(out VCA vca)) {
                vca.getVolume(out volume);
            }
            return volume;
        }

        private static float GetSoundVolume() {
            ValidateInitializationState();
            float volume = FALLBACK_VOLUME;
            if(TryGetSoundVCA(out VCA vca)) {
                vca.getVolume(out volume);
            }
            return volume;
        }

        private static void SetMusicVolume(float volume) {
            ValidateInitializationState();
            if(!TryGetMusicVCA(out VCA vca)) {
                return;
            }
            vca.setVolume(volume);
            Logger.WriteLine($"Set music volume to {volume}.",LoggerLabel.Audio);
        }

        private static void SetSoundVolume(float volume) {
            ValidateInitializationState();
            if(!TryGetSoundVCA(out VCA vca)) {
                return;
            }
            vca.setVolume(volume);
            Logger.WriteLine($"Set sound volume to {volume}.",LoggerLabel.Audio);
        }

    }
}
