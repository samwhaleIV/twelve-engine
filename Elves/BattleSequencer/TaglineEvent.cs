using System;
using System.Collections.Generic;
using System.Text;

namespace Elves.BattleSequencer {
    public class TaglineEvent:UVEvent {
        private readonly string message;

        public TaglineEvent(string message) {
            this.message = message;
        }

        public TaglineEvent(string message,UVUser user) {
            this.message = string.Format(message,user.Name);
        }

        public TaglineEvent(string message,UVUser user,int value) {
            this.message = string.Format(message,user.Name,value);
        }

        public override void Invoke(UVSequencer uvSequencer) {
            uvSequencer.EmitTagline(message);
        }
    }
}
