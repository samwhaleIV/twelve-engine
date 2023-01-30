using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;

namespace TwelveEngine.Input {
    public static class KeyBinds {

        private static Dictionary<Impulse,Keys> GetBinds(KeyBindSet keyBindSet) => new() {
            { Impulse.Up, keyBindSet.Up },
            { Impulse.Down, keyBindSet.Down },
            { Impulse.Left, keyBindSet.Left },
            { Impulse.Right, keyBindSet.Right },

            { Impulse.Accept, keyBindSet.Accept },
            { Impulse.Cancel, keyBindSet.Cancel },

            { Impulse.Ascend, keyBindSet.Ascend },
            { Impulse.Descend, keyBindSet.Descend },

            { Impulse.Focus, keyBindSet.Focus }
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

        private static readonly Dictionary<Impulse,Keys> impulses = GetBinds(new KeyBindSet());

        public static Keys Get(Impulse type) {
            return impulses[type];
        }

        public static bool TryGet(Impulse type,out Keys key) {
            var hasImpulse = impulses.TryGetValue(type, out key);
            if(!hasImpulse) {
                key = Keys.None;
            }
            return hasImpulse;
        }

        public static bool HasImpulse(Impulse type) {
            return impulses.ContainsKey(type);
        }

        public static void Set(Impulse type,Keys key) {
            if(!validKeys.Contains(key)) {
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
                writer.Write((int)key);
            }
        }

        public static void Import(BinaryReader reader) {
            int count = reader.ReadInt32();
            for(var i = 0;i<count;i++) {
                var impulse = (Impulse)reader.ReadInt32();
                var key = (Keys)reader.ReadInt32();
                if(!validKeys.Contains(key)) {
                    continue;
                }
                impulses[impulse] = key;
            }
        }

        private static readonly Keys[] ValidKeysList = new Keys[] {
            Keys.OemTilde,
            Keys.D1,
            Keys.D2,
            Keys.D3,
            Keys.D4,
            Keys.D5,
            Keys.D6,
            Keys.D7,
            Keys.Tab,
            Keys.Q,
            Keys.W,
            Keys.E,
            Keys.R,
            Keys.T,
            Keys.Y,
            Keys.U,
            Keys.Space,
            Keys.A,
            Keys.S,
            Keys.D,
            Keys.F,
            Keys.G,
            Keys.H,
            Keys.J,
            Keys.LeftShift,
            Keys.Z,
            Keys.X,
            Keys.C,
            Keys.V,
            Keys.B,
            Keys.N,
            Keys.M,
            Keys.Up,
            Keys.Down,
            Keys.Left,
            Keys.Right,
            Keys.Insert,
            Keys.Home,
            Keys.PageUp,
            Keys.PageDown,
            Keys.D8,
            Keys.D9,
            Keys.D0,
            Keys.OemMinus,
            Keys.OemPlus,
            Keys.Back,
            Keys.OemComma,
            Keys.OemPeriod,
            Keys.I,
            Keys.O,
            Keys.P,
            Keys.OemOpenBrackets,
            Keys.OemCloseBrackets,
            Keys.OemBackslash,
            Keys.OemQuestion,
            Keys.LeftControl,
            Keys.K,
            Keys.L,
            Keys.OemSemicolon,
            Keys.OemQuotes,
            Keys.Enter,
            Keys.Delete,
            Keys.End,
            Keys.Escape
        };

        private static readonly HashSet<Keys> validKeys = GetValidKeysSet();

        private static HashSet<Keys> GetValidKeysSet() {
            HashSet<Keys> hashSet = new();
            Keys[] list = ValidKeysList;
            foreach(var key in list) {
                hashSet.Add(key);
            }
            return hashSet;
        }
    }
}
