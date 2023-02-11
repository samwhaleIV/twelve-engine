using TwelveEngine.Shell;
using System;
using TwelveEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using TwelveEngine.Audio;

namespace Elves {
    public static class Program {

        public static GameStateManager Game { get; private set; }
        public static Textures Textures { get; private set; }

        public static SaveFile Save { get; set; } = null;
        public static SaveFile GlobalSave { get; private set; } = null;

        public static readonly List<SaveFile> Saves = new();

        private static void LoadSaveFiles() {
            if(Saves.Count > 0) {
                throw new InvalidOperationException("Local save files have already been loaded.");
            }
            for(int i = 0;i<3;i++) {
                SaveFile saveFile = new() {
                    Path = Path.Combine(SaveDirectory,string.Format(Constants.SaveFileFormat,i + 1))
                };
                Saves.Add(saveFile);
                saveFile.TryLoad();
            }
        }

        private static void LoadGlobalSaveFile() {
            SaveFile globalSaveFile = new() {
                Path = Path.Combine(SaveDirectory,Constants.GlobalSaveFile)
            };
            globalSaveFile.TryLoad();
            GlobalSave = globalSaveFile;
        }

        public static string SaveDirectory { get; private set; }

        public static void Start(GameStateManager game,string saveDirectory) {
            Game = game;

            Game.Window.Title = "Elves!";

            Textures.ContentManager = Game.Content;
            Textures = new Textures();

            if(!Flags.Get(Constants.Flags.OSCursor)) {
                AddCustomCursors();
                CustomCursor.UseCustomCursor = true;
            }

            SaveDirectory = saveDirectory;

            LoadSaveFiles();
            LoadGlobalSaveFile();
            LoadAudioBanks();
            SetupVolume();

            game.SetState(ElfGame.Start());
        }

        public static void OnGameCrashed() {
            Save?.TrySave();
        }

        private static void AddCursorState(
            CursorState cursorState,Texture2D texture,int? originX = null,int? originY = null
        ) {
            CustomCursor.Sources.Add(cursorState,MouseCursor.FromTexture2D(texture,originX ?? 0,originY ?? 0));
        }

        public static BankWrapper AudioBank { get; private set; }
        public static bool HasAudioBank { get; private set; }

        private static void SetupVolume() {
            GlobalSave.TryGetFloat(SaveKeys.SoundVolume,out float soundVolume,Constants.DefaultSoundVolume);
            AudioSystem.SoundVolume = soundVolume;
            GlobalSave.SetValue(SaveKeys.SoundVolume,soundVolume);

            GlobalSave.TryGetFloat(SaveKeys.MusicVolume,out float musicVolume,Constants.DefaultMusicVolume);
            AudioSystem.MusicVolume = musicVolume;
            GlobalSave.SetValue(SaveKeys.MusicVolume,musicVolume);
        }

        private static void LoadAudioBanks() {
            AudioSystem.TryLoadBank(Config.GetString(Config.Keys.FMODStrings),out _);
            AudioSystem.TryLoadBank(Config.GetString(Config.Keys.FMODMaster),out _);

            HasAudioBank = AudioSystem.TryLoadBank(Config.GetString(Config.Keys.FMODGame),out var audioBank);
            AudioBank = HasAudioBank ? audioBank : default;

            AudioSystem.BindVCAs();
        }

        private static void AddCustomCursors() {
            AddCursorState(CursorState.Default,Textures.CursorDefault,32,32);
            AddCursorState(CursorState.Interact,Textures.CursorAlt1,32,32);
            AddCursorState(CursorState.Pressed,Textures.CursorAlt2,32,32);
        }
    }
}
