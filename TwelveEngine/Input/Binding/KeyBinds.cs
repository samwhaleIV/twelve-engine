using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input.Binding {
    public static class KeyBinds {

        private static Dictionary<Impulse,MultiBindKey> GetBinds(KeyBindSet keyBindSet) => new() {
            { Impulse.Up, keyBindSet.Up },
            { Impulse.Down, keyBindSet.Down },
            { Impulse.Left, keyBindSet.Left },
            { Impulse.Right, keyBindSet.Right },

            { Impulse.Accept, keyBindSet.Accept },
            { Impulse.Cancel, keyBindSet.Cancel },

            { Impulse.Ascend, keyBindSet.Ascend },
            { Impulse.Descend, keyBindSet.Descend },

            { Impulse.Focus, keyBindSet.Focus },
            { Impulse.Debug, keyBindSet.Debug }
        };

        public static Dictionary<Impulse,Buttons> GetControllerBinds() => new() {
            { Impulse.Up, Buttons.DPadUp },
            { Impulse.Down, Buttons.DPadDown },
            { Impulse.Left, Buttons.DPadLeft },
            { Impulse.Right, Buttons.DPadRight },

            { Impulse.Accept, Buttons.A },
            { Impulse.Cancel, Buttons.B },

            { Impulse.Ascend, Buttons.LeftShoulder },
            { Impulse.Descend, Buttons.RightShoulder },
            { Impulse.Focus, Buttons.Y }
        };

        public static string Path { get; set; }

        private static readonly Dictionary<Impulse,MultiBindKey> impulses = GetBinds(new KeyBindSet());

        public static MultiBindKey Get(Impulse type) {
            return impulses[type];
        }

        public static bool TryGet(Impulse type,out MultiBindKey key) {
            var hasImpulse = impulses.TryGetValue(type, out key);
            if(!hasImpulse) {
                key = MultiBindKey.None;
            }
            return hasImpulse;
        }

        public static bool HasImpulse(Impulse type) {
            return impulses.ContainsKey(type);
        }

        public static void Set(Impulse type,MultiBindKey key) {
            if(!ValidKeys.Contains(ref key)) {
                return;
            }
            impulses[type] = key;
        }

        public static bool TrySave() {
            bool success = false;
            try {
                using var stream = File.OpenWrite(Path);
                using var writer = new BinaryWriter(stream);
                Export(writer);
                writer.Close();
                success = true;
            } catch(Exception exception) {
                Logger.WriteLine($"Failure saving key binds: {exception}",LoggerLabel.KeyBinds);
            }
            if(success) {
                Logger.WriteLine($"Saved key binds to \"{Path}\"",LoggerLabel.KeyBinds);
            }
            return success;
        }

        public static bool TryLoad() {
            bool success = false;
            if(!File.Exists(Path)) {
                Logger.WriteLine("Key binds file does not exist, loading defaults.",LoggerLabel.KeyBinds);
                return false;
            }
            try {
                using var stream = File.OpenRead(Path);
                using var reader = new BinaryReader(stream);
                Import(reader);
                reader.Close();
                success = true;
            } catch(Exception exception) {
                Logger.WriteLine($"Failed to load key binds from Path \"{Path}\": {exception}",LoggerLabel.KeyBinds);
            }
            if(success) {
                Logger.WriteLine($"Loaded key binds from Path \"{Path}\".",LoggerLabel.KeyBinds);
            }
            return success;
        }

        public static void Export(BinaryWriter writer) {
            writer.Write(impulses.Count);
            foreach(var (impulse,key) in impulses) {
                writer.Write((int)impulse);
                key.Write(writer);
            }
        }

        public static void Import(BinaryReader reader) {
            int count = reader.ReadInt32();
            for(var i = 0;i<count;i++) {
                var impulse = (Impulse)reader.ReadInt32();
                MultiBindKey key = MultiBindKey.Read(reader);
                if(!ValidKeys.Contains(ref key)) {
                    continue;
                }
                impulses[impulse] = key;
            }
        }

    }
}
