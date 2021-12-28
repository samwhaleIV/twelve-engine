using System.Collections.Generic;

namespace TwelveEngine.Game2D {
    public sealed class InteractionLayer {

        private readonly List<IInteract> targets = new List<IInteract>();
        public void AddTarget(IInteract target) => targets.Add(target);

        public void HitTest(Entity2D source) {
            var interactionBox = source.GetInteractionBox();
            foreach(var target in targets) {
                if(interactionBox.Collides(target.GetHitbox())) {
                    target.Interact(source);
                    break;
                }
            }
        }
    }
}
