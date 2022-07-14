using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Collision;
using TwelveEngine.Game2D.Entity;

namespace Porthole.PuzzleGame {
    public sealed class PuzzleGrid:TileGrid {
        
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
