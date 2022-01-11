using TwelveEngine;

namespace ElfGame.ElfSprite.Face.Controller {
    internal abstract class Component:TimeoutThread {
        public FaceState State { get; set; }
    }
}
