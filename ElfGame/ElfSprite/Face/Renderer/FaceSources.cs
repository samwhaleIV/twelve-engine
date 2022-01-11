using Microsoft.Xna.Framework;

namespace ElfGame.ElfSprite.Face {
    internal sealed class FaceSources {
        public readonly Rectangle FaceBackground = new Rectangle(11,1,9,9);

        public readonly Rectangle MouthClosed = new Rectangle(1,11,3,2);
        public readonly Rectangle MouthOpen = new Rectangle(5,11,3,2);
        public readonly Rectangle MouthSad = new Rectangle(1,14,3,2);
        public readonly Rectangle MouthScream = new Rectangle(9,11,3,2);

        public readonly Rectangle EyesNormal = new Rectangle(13,11,5,2);
        public readonly Rectangle EyesDazed = new Rectangle(13,14,5,2);
        public readonly Rectangle EyesSly = new Rectangle(13,17,5,2);
        public readonly Rectangle EyesUncaring = new Rectangle(13,20,5,2);

        public readonly Rectangle NormalEyelid = new Rectangle(19,11,2,2);

        public readonly Rectangle BruisedEyelid = new Rectangle(22,11,2,2);

        public readonly Rectangle LeftEyeTears = new Rectangle(19,14,2,2);
        public readonly Rectangle RightEyeTears = new Rectangle(22,14,2,2);

        public readonly Rectangle EyeBruise = new Rectangle(19,17,2,1);
    }
}
