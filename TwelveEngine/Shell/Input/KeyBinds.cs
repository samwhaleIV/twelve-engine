using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using TwelveEngine.Shell.Input.Glyphs;
using TwelveEngine.Serial;
using TwelveEngine.Shell.Config;

namespace TwelveEngine.Shell.Input {
    public sealed partial class KeyBinds:ISerializable {

        private readonly Dictionary<Impulse,Keys> binds;
        private readonly KeyBindSet defaultSet;

        private KeyBinds(KeyBindSet keyBindSet) {
            defaultSet = keyBindSet;
            var invalidBinds = GetBinds(keyBindSet);
            this.binds = validateBinds(invalidBinds);
        }

        private readonly HashSet<Keys> validKeys = getValidKeysSet();
        private readonly HashSet<Keys> existingKeys = new HashSet<Keys>();

        private static HashSet<Keys> getValidKeysSet() {
            var hashSet = new HashSet<Keys>();
            var list = KeyboardMap.GetKeys();

            foreach(var key in list) {
                hashSet.Add(key);
            }
            return hashSet;
        }

        /* Verifies that the key is valid (i.e. we have a glyph for it) and that there are no duplicate entries */
        public bool IsKeyValid(Keys key) {
            return validKeys.Contains(key) && !existingKeys.Contains(key);
        }
        public bool IsKeyValid(Keys key,HashSet<Keys> existingKeys) {
            return validKeys.Contains(key) && !existingKeys.Contains(key);
        }

        private Dictionary<Impulse,Keys> validateBinds(Dictionary<Impulse,Keys> binds) {
            var existingKeys = new HashSet<Keys>();
            foreach(var bind in binds) {
                var key = bind.Value;
                if(!IsKeyValid(key,existingKeys)) {
                    return GetBinds(new KeyBindSet());
                }
                existingKeys.Add(key);
            }
            return binds;
        }

        private void setKey(Impulse impulse,Keys value) {
            if(!IsKeyValid(value)) {
                return;
            }
            var oldValue = binds[impulse];
            existingKeys.Remove(oldValue);

            existingKeys.Add(value);
            binds[impulse] = value;
        }

        public Keys this[Impulse type] {
            get => binds[type];
            set => setKey(type,value);
        }

        public void Save(string path = null) {
            var keyBindSet = defaultSet;

            keyBindSet.Up = this[Impulse.Up];
            keyBindSet.Down = this[Impulse.Down];
            keyBindSet.Left = this[Impulse.Left];
            keyBindSet.Right = this[Impulse.Right];
            keyBindSet.Accept = this[Impulse.Accept];
            keyBindSet.Cancel = this[Impulse.Cancel];

            ConfigWriter.SaveKeyBinds(keyBindSet,path);
        }

        public static KeyBinds Load(string path = null) {
            var keyBindSet = ConfigLoader.LoadKeyBinds(path);
            return new KeyBinds(keyBindSet);
        }

        internal static KeyBinds Load(out KeyBindSet keyBindSet,string path = null) {
            keyBindSet = ConfigLoader.LoadKeyBinds(path);
            return new KeyBinds(keyBindSet);
        }

        public void Export(SerialFrame frame) {
            frame.Set(binds.Count);
            foreach(var bind in binds) {
                frame.Set(bind.Key);
                frame.Set(bind.Value);
            }
        }

        public void Import(SerialFrame frame) {
            int count = frame.GetInt();
            for(var i = 0;i<count;i++) {
                var bind = frame.GetBind();
                var key = frame.GetKey();
                binds[bind] = key;
            }
        }
    }
}
