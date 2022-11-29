using System;
using System.Collections.Generic;
using System.Text;

namespace Elves.BattleSequencer {
    public class User {
        public int Health { get; set; } = Constants.DefaultHealth;
        public int MaxHealth { get; set; } = Constants.DefaultHealth;
        public string Name { get; set; } = Constants.NoName;

        public bool IsPlayer { get; set; } = false;

        private readonly Dictionary<string,string> properties = new Dictionary<string,string>();

        public bool HasValue(string name) {
            return properties.ContainsKey(name);
        }
        public string GetValue(string name) {
            if(!properties.ContainsKey(name)) {
                return null;
            } else {
                return properties[name];
            }
        }
        public void SetValue(string name,string value) {
            properties[name] = value;
        }
        public bool DeleteValue(string name) {
            return properties.Remove(name);
        }
    }
}

