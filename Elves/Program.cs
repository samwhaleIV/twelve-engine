using System;
using System.Text;
using TwelveEngine.Shell;
using Elves.Battle;
using System.IO;

namespace Elves {
    public sealed class Program {

        private readonly string saveDirectory;
        private readonly string saveFilePath;

        public string SaveDirectory => saveDirectory;
        public string SaveFilePath => saveFilePath;

        private readonly SaveData saveData = new SaveData();

        private readonly GameManager game;
        public GameManager Game => game;

        private readonly bool shouldCreateDirectory;

        public Program(GameManager game,string saveDirectory,bool shouldCreateDirectory) {
            this.shouldCreateDirectory = shouldCreateDirectory;

            this.saveDirectory = saveDirectory;
            saveFilePath = Path.Combine(saveDirectory,"elves.save");

            LoadSaveData();
            if(saveData.KeyCount <= 0) {
                Console.WriteLine("Heads up: Starting game from a blank save slate.");
            }
            SaveSaveData();

            game.SetState(new BattleScene("Backgrounds/checkerboard"));
        }

        private bool notifiedLackOfDirectoryCreationAuthority = false;

        private bool CheckOrCreateSaveDirectory(StringBuilder stringBuilder) {
            if(Directory.Exists(SaveDirectory)) {
                return true;
            } else if(!shouldCreateDirectory) {
                if(!notifiedLackOfDirectoryCreationAuthority) {
                    stringBuilder.AppendLine("Engine has specified a lack of authority to create a new save directory, but if you are so inclined, you may do this yourself.");
                    notifiedLackOfDirectoryCreationAuthority = true;
                }
                return false;
            }
            bool success;
            try {
                Directory.CreateDirectory(saveDirectory);
                stringBuilder.AppendLine($"Created save directory \"{saveDirectory}\" because it did not exist.");
                success = true;
            } catch(Exception exception) {
                stringBuilder.AppendLine($"Failure to create save directory \"{saveDirectory}\": {exception}");
                success = false;
            }
            return success;
        }

        private void LoadSaveData() {
            StringBuilder stringBuilder = new StringBuilder();
            if(!saveData.TryLoad(saveFilePath,stringBuilder)) {
                Console.Write($"Save data load failure: {stringBuilder}");
            } else {
                Console.Write($"Save data load success: {stringBuilder}");
            }
        }

        private void SaveSaveData() {
            StringBuilder stringBuilder = new StringBuilder();
            if(!CheckOrCreateSaveDirectory(stringBuilder)) {
                stringBuilder.AppendLine("Save data directory does not exist, cannot write save file.");
                Console.WriteLine(stringBuilder);
                return;
            }
            if(!saveData.TrySave(saveFilePath,stringBuilder)) {
                Console.Write($"Save data write failure: {stringBuilder}");
            } else {
                Console.Write($"Save data write success: {stringBuilder}");
            }
        }
    }
}
