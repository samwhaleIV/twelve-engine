using System;
using System.Collections.Generic;
using System.Text;

namespace Elves.BattleSequencer {
    public abstract class UVSequencer {
        private Dictionary<int,User> users = new Dictionary<int,User>();

        protected Action<User> UserAdded, UserRemoved;

        public bool AddUser(int ID,User user) {
            if(!users.ContainsKey(ID)) {
                return false;
            }
            users.Add(ID,user);
            UserAdded?.Invoke(user);
            return true;
        }

        public bool RemoveUser(int ID) {
            if(!users.ContainsKey(ID)) {
                return false;
            }
            User user = users[ID];
            users.Remove(ID);
            UserRemoved?.Invoke(user);
            return true;
        }

        public void ProcessUsers() {

        }
    }
}
