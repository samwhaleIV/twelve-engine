using System.Collections.Generic;
using TwelveEngine.Game2D;
using Porthole.Collision;
using TwelveEngine.Game2D.Entity;

namespace Porthole.PuzzleGame {
    public sealed class PuzzleGrid:TileGrid {

        public TileCollision Collision { get; private set; }

        public PuzzleGrid() {
            Collision = new TileCollision(this);
            OnLoad += PuzzleGrid_OnLoad;
        }

        private void PuzzleGrid_OnLoad() {
            CollisionTypes?.Load();
        }

        private TileCollisionTypes _collisionTypes;

        public TileCollisionTypes CollisionTypes {
            get => _collisionTypes;
            set {
                if(_collisionTypes == value) return;
                if(IsLoaded && !value.IsLoaded) value.Load();
                _collisionTypes = value;
            }
        }

        private readonly List<IInteract> targets = new List<IInteract>();

        public void AddHitTarget(IInteract target) => targets.Add(target);
        public void RemoveHitTarget(IInteract target) => targets.Remove(target);

        public void TestHitTargets(Entity2D source) {
            var interactionBox = Hitbox.GetInteractionArea(source.Position,source.Size,source.Direction);
            foreach(var target in targets) {
                if(interactionBox.Collides(target.GetHitbox())) {
                    target.Interact(source);
                    break;
                }
            }
        }
    }
}
