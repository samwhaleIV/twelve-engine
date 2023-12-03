using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TwelveEngine;

namespace John {
    public sealed class PoolOfJohns:PoolSet<WalkingJohn> {

        private JohnCollectionGame _game;

        public WalkingJohn LeaseJohn(int configID,JohnStartPosition startPosition) {
            int poolID = Lease(out WalkingJohn john);
            john.Enable(poolID,startPosition);
            john.ConfigID = configID;
            return john;
        }

        public void ReturnJohn(WalkingJohn john) {
            Return(john.PoolID);
            john.Disable();
        }

        public PoolOfJohns(JohnCollectionGame game) {
            _game = game;
        }

        protected override WalkingJohn CreateNew() {
            var john = new WalkingJohn(_game);
            _game.Entities.Add(john);
            john.Disable();
            return john;
        }

        protected override void Reset(WalkingJohn item) {
            item.Disable();
        }
    }
}
