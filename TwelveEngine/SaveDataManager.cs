using System;
using System.Text;
using System.IO;

namespace TwelveEngine {
    public static class SaveDataManager {

        private static string _saveDirectory;
        private static string _saveFilePath;

        private static bool _shouldCreateDirectory;

        public static string SaveDirectory => _saveDirectory;
        public static string SaveFilePath => _saveFilePath;

        private static SaveData _saveData = new SaveData();

        public static SaveData Data {
            get {
                return _saveData;
            }
            set {
                _saveData = value;
            }
        }

        private static bool notifiedLackOfDirectoryCreationAuthority = false;

        private static readonly StringBuilder stringBuilder = new StringBuilder();

        public static void Initialize(string saveFile,string saveDirectory,bool shouldCreateDirectory) {
            _shouldCreateDirectory = shouldCreateDirectory;

            _saveDirectory = saveDirectory;
            _saveFilePath = Path.Combine(saveDirectory,saveFile);

            Load();
            if(_saveData.KeyCount <= 0) {
                Logger.WriteLine("Heads up: Starting game from a blank save slate.");
            }
            Save(); /* Rewrite the save data to make sure formatting is up to date ...
                     * If formatting ever changes for some reason. If anything, this is a quick test to see if save data works at all. */
        }

        private static bool CheckOrCreateSaveDirectory(StringBuilder stringBuilder) {
            if(Directory.Exists(SaveDirectory)) {
                return true;
            } else if(!_shouldCreateDirectory) {
                if(!notifiedLackOfDirectoryCreationAuthority) {
                    stringBuilder.AppendLine("Platform has specified a lack of authority to create a new save directory, but if you are so inclined, you may do this yourself.");
                    notifiedLackOfDirectoryCreationAuthority = true;
                }
                return false;
            }
            bool success;
            try {
                Directory.CreateDirectory(_saveDirectory);
                stringBuilder.AppendLine($"Created save directory \"{_saveDirectory}\" because it did not exist.");
                success = true;
            } catch(Exception exception) {
                stringBuilder.AppendLine($"Failure to create save directory \"{_saveDirectory}\": {exception}");
                success = false;
            }
            return success;
        }

        public static void Load() {
            stringBuilder.Clear();
            if(!_saveData.TryLoad(_saveFilePath,stringBuilder)) {
                Logger.Write("Save data load failure: ");
                Logger.WriteLine(stringBuilder);
            } else {
                Logger.Write("Save data load success: ");
                Logger.WriteLine(stringBuilder);
            }
        }

        public static void Save() {
            stringBuilder.Clear();
            if(!CheckOrCreateSaveDirectory(stringBuilder)) {
                Logger.Write("Save data directory does not exist, cannot write save file. ");
                Logger.WriteLine(stringBuilder);
                return;
            }
            if(!_saveData.TrySave(_saveFilePath,stringBuilder)) {
                Logger.Write($"Save data write failure: ");
                Logger.WriteLine(stringBuilder);
            } else {
                Logger.Write($"Save data write success: ");
                Logger.WriteLine(stringBuilder);
            }
        }
    }
}
