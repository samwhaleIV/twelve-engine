using Microsoft.Xna.Framework;

namespace ElfGame.ElfSprite.Face {
    internal sealed class FaceTargets {
        public readonly Rectangle LeftBruise = new Rectangle(2,3,2,1);
        public readonly Rectangle RightBruise = new Rectangle(5,3,2,1);

        public readonly Rectangle LeftTear = new Rectangle(2,3,2,2);
        public readonly Rectangle RightTear = new Rectangle(5,3,2,2);

        public readonly Rectangle Mouth = new Rectangle(3,5,3,2);
        public readonly Rectangle Eyes = new Rectangle(2,1,5,2);

        public readonly Rectangle LeftEyelid = new Rectangle(2,1,2,2);
        public readonly Rectangle RightEyelid = new Rectangle(5,1,2,2);
    }
}
