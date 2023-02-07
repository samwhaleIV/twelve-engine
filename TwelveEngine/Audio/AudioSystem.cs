﻿using FMOD.Studio;

namespace TwelveEngine.Audio {
    public static class AudioSystem {

        private const int DEFAULT_SAMPLE_RATE = 48000;
        private const int MAX_STUDIO_CHANNELS = 32;

        private static bool _initialized = false, _disposed = false;

        private static FMOD.Studio.System StudioSystem;
        private static FMOD.System CoreSystem;

        private static int _bankIDCounter = 0;
        private static readonly Dictionary<string,int> BankIDs = new();
        private static readonly Dictionary<int,BankWrapper> Banks = new();

        public static IEnumerable<BankWrapper> GetBanks() => Banks.Values;

        private static void ValidateInitializationState() {
            if(_initialized) {
                return;
            }
            throw new InvalidOperationException("Audio controller has not been initialized.");
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

        public static BankWrapper LoadBank(string bankFile) {
            ValidateInitializationState();
            int bankID = GetBankID(bankFile);
            if(Banks.ContainsKey(bankID)) {
                throw new InvalidOperationException($"Bank '{bankFile}' (ID: {bankID}) has already been loaded.");
            }
            /* 'LOAD_BANK_FLAGS.NORMAL' is blocking. This way, we cannot return use that has not been fully loaded */
            StudioSystem.loadBankFile(bankFile,LOAD_BANK_FLAGS.NORMAL,out Bank bank);
            BankWrapper bankWrapper = new(bank,bankID,bankFile);
            Banks.Add(bankID,bankWrapper);
            return bankWrapper;
        }

        public static bool HasBank(int bankID) => Banks.ContainsKey(bankID);

        public static void UnloadBank(int bankID) {
            ValidateInitializationState();
            if(!Banks.ContainsKey(bankID)) {
                throw new InvalidOperationException($"Bank '{bankID}' does not exist or has already been unloaded.");
            }
            Bank bank = Banks[bankID].Bank;
            Banks.Remove(bankID);
            bank.unload();
        }

        public static bool TryGetBank(int bankID,out BankWrapper bank) => Banks.TryGetValue(bankID,out bank);

        /// <summary>
        /// Cleanup the underlying FMOD audio system. Must been called before the game terminates.<br/>
        /// I honestly don't know what happens if you don't, but it probably doesn't make the operating system very happy.<br/><br/>
        /// Cleanup includes previously loaded audio banks contained in <see cref="Banks"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        internal static void Unload() {
            ValidateInitializationState();
            if(_disposed) {
                throw new InvalidOperationException("Audio controller has already been disposed.");
            }
            _disposed = true;
            foreach(var bank in Banks.Values) {
                bank.Bank.unload();
            }
            Banks.Clear();
            StudioSystem.release();
            /* The core audio system is automatically released when the studio system is. */;
        }
    }
}
