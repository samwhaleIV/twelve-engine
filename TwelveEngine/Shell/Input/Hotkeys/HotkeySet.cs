using Microsoft.Xna.Framework.Input;
using System;

namespace TwelveEngine.Shell.Input {
    public sealed class HotkeySet {
        private readonly Hotkey[] set;
        public HotkeySet(params (Keys key,Action action)[] set) {
            var newSet = new Hotkey[set.Length];
            for(var i = 0;i<set.Length;i++) {
                var item = set[i];
                var action = item.action;
                newSet[i] = new Hotkey(item.key,action);
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
