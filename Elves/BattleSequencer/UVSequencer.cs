using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TwelveEngine.Serial;
using System.Threading.Tasks;

namespace Elves.BattleSequencer {
    public abstract class UVSequencer {

        public User CreatePlayer() {
            var player = new User() {
                Name = StorageSystem.GetPlayerName(),
                IsPlayer = true
            };
            return player;
        }

        public User CreateElf() {
            var elf = new User() {
                IsPlayer = false
            };
            return elf;
        }

        public abstract Task Tag(string text);
        public abstract Task SetActor(User user);
        public abstract Task Speech(string text);
        public abstract Task<int> SelectOption(string[] options);

        public abstract Task AddUser(User user);
        public abstract Task RemoveUser(User user);
        public abstract Task UserHurt(User user);
        public abstract Task UserHealed(User user);
    }
}
