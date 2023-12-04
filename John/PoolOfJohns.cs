using nkast.Aether.Physics2D.Dynamics;
using System.Collections.Generic;
using TwelveEngine;

namespace John {
    public sealed class PoolOfJohns:PoolSet<WalkingJohn> {

        private readonly CollectionGame _game;

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

        public PoolOfJohns(CollectionGame game) {
            _game = game;
        }

        private readonly Dictionary<Fixture,WalkingJohn> _fixtureTable = new Dictionary<Fixture,WalkingJohn>();

        protected override WalkingJohn CreateNew() {
            var john = new WalkingJohn(_game);
            _game.Entities.Add(john);
            _fixtureTable[john.Fixture] = john;
            john.Disable();
            return john;
        }

        public bool TryGetJohn(Fixture fixture,out WalkingJohn john) {
            return _fixtureTable.TryGetValue(fixture, out john);
        }

        protected override void Reset(WalkingJohn item) {
            item.Disable();
        }
    }
}
