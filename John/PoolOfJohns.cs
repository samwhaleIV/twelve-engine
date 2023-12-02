using System.Numerics;
using TwelveEngine;

namespace John {
    public sealed class PoolOfJohns:PoolSet<WalkingJohn> {

        private JohnCollectionGame _game;

        public WalkingJohn LeaseJohn(Vector2 position,int configID) {
            int poolID = Lease(out WalkingJohn john);
            john.Enable(poolID,position);
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
            var john = new WalkingJohn(_game.JohnDecorator);
            _game.Entities.Add(john);
            return john;
        }

        protected override void Reset(WalkingJohn item) {
            item.Disable();
        }
    }
}
