using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TwelveEngine.Serial;

namespace Elves.BattleSequencer {
    public abstract class UVSequencer:ISerializable {

        internal abstract void EmitTagline(string message);
        internal abstract void EmitUserDeath(UVUser user);
        internal abstract void EmitUserEnterBattle(UVUser user);
        internal abstract void EmitUserExitBattle(UVUser user);
        internal abstract void EmitUserDamaged(UVUser user,int amount);
        internal abstract void EmitUserHealed(UVUser user,int amount);
        internal abstract void EmitUserSpeech(UVUser user,string message);

        private Dictionary<int,UVUser> users = new Dictionary<int,UVUser>();
        private bool _userListLocked = false;
        private void LockUserList() => _userListLocked = true;
        private void UnlockUserList() => _userListLocked = false;
        private Queue<UVUser> newUserQueue = new Queue<UVUser>(), removedUserQueue = new Queue<UVUser>();
        private int _ID = 0;
        private int GetNewID() => _ID++;

        public UVUser AddUser(UVUser user) {
            if(user._ID > Constants.InvalidUserID) {
                /* Cannot be added; The user is already added to the dictionary */
                return user;
            }
            int ID = GetNewID();
            user._ID = ID;
            if(_userListLocked) {
                newUserQueue.Enqueue(user);
                return user;
            }
            users.Add(user._ID,user);
            user.GetEnterEvent().Invoke(this);
            return user;
        }

        public UVUser RemoveUser(UVUser user) {
            if(!users.ContainsKey(user._ID)) {
                /* Cannot be removed; The user is not in the dictionary */
                return user;
            }
            if(_userListLocked) {
                removedUserQueue.Enqueue(user);
                user._InRemovalQueue = true;
                return user;
            }
            users.Remove(user._ID);
            user.GetExitEvent().Invoke(this);
            return user;
        }

        public void ProcessEvent(UVEvent uvEvent) => uvEvent?.Invoke(this);

        public void ProcessUserTurns() {
            LockUserList();
            KeyValuePair<int,UVUser>[] usersList = users.OrderBy(key => key.Value.Priority).ToArray();
            if(usersList.Length <= 0) {
                Events.NobodyIsHere().Invoke(this);
            }
            foreach(var item in usersList) {
                UVUser user = item.Value;
                if(user.IsDead || user._InRemovalQueue) {
                    continue;
                }
                if(user.Modifiers.HasFlag(UserModifiers.Disabled)) {
                    Events.UserDisabled(user).Invoke(this);
                    continue;
                }
                user.GetTurnEvent().Invoke(this);
            }
            foreach(var item in usersList) {
                UVUser user = item.Value;
                if(user.IsDead) {
                    RemoveUser(user);
                }
            }
            UnlockUserList();
            foreach(UVUser user in removedUserQueue) {
                users.Remove(user._ID);
                user._ID = -1;
                user._InRemovalQueue = false;
                if(!user.IsDead) {
                    user.GetExitEvent().Invoke(this);
                }
            }
            removedUserQueue.Clear();
            foreach(UVUser user in newUserQueue) {
                users.Add(user._ID,user);
                user.GetEnterEvent().Invoke(this);
            }
            newUserQueue.Clear();
        }

        public void Export(SerialFrame frame) {
            throw new NotImplementedException();
        }

        public void Import(SerialFrame frame) {
            throw new NotImplementedException();
        }
    }
}
