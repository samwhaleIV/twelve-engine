using Microsoft.Xna.Framework.Input;
using System;

namespace TwelveEngine.Input {
    public sealed class KeyWatcherSet {
        private readonly KeyWatcher[] set;
        public KeyWatcherSet(params (Keys key,Action action)[] set) {
            var newSet = new KeyWatcher[set.Length];
            for(var i = 0;i<set.Length;i++) {
                var item = set[i];
                var action = item.action;
                newSet[i] = new KeyWatcher(item.key,action);
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
