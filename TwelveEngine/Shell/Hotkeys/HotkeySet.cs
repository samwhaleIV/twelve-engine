using Microsoft.Xna.Framework.Input;
using System;

namespace TwelveEngine.Shell.Hotkeys {
    public sealed class HotkeySet {
        private readonly Hotkey[] set;
        public HotkeySet(params (Keys Key,Action Action)[] set) {
            var newSet = new Hotkey[set.Length];
            for(var i = 0;i<set.Length;i++) {
                var item = set[i];
                var action = item.Action;
                newSet[i] = new Hotkey(item.Key,action);
            }
            this.set = newSet;
        }
        public void Update(KeyboardState keyboardState) {
            foreach(var keyWatcher in set) {
                keyWatcher.Update(keyboardState);
            }
        }
    }
}
