using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elves.BattleSequencer {
    public sealed class ConsoleBattleInterface:UVSequencer {
        public override Task AddUser(User user) {
            throw new NotImplementedException();
        }

        public override Task RemoveUser(User user) {
            throw new NotImplementedException();
        }

        public override Task<int> SelectOption(string[] options) {
            throw new NotImplementedException();
        }

        public override Task SetActor(User user) {
            throw new NotImplementedException();
        }

        public override Task Speech(string text) {
            throw new NotImplementedException();
        }

        public override Task Tag(string text) {
            throw new NotImplementedException();
        }

        public override Task UserHealed(User user) {
            throw new NotImplementedException();
        }

        public override Task UserHurt(User user) {
            throw new NotImplementedException();
        }
    }
}
