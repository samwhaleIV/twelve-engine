using System;

namespace ElfGame.ElfSprite.Face {
    internal sealed class FaceState {

        public EyeBruising EyeBruising { get; set; } = EyeBruising.None;
        public EyeTears EyeTears { get; set; } = EyeTears.None;

        public EyeType EyeType { get; set; } = EyeType.Normal;
        public EyeBlink EyeBlink { get; set; } = EyeBlink.None;

        public MouthType MouthType { get; set; } = MouthType.Normal;
        public MouthMode MouthMode { get; set; } = MouthMode.Idle;

        public static FaceState GetRandom(Random random) => new FaceState() {
            /* Does not use reflection, maximum values may drift as more types are added */

            EyeBruising = (EyeBruising)random.Next(0,(int)EyeBruising.All),
            EyeTears = (EyeTears)random.Next(0,(int)EyeTears.All),

            EyeType = (EyeType)random.Next(0,(int)EyeType.None),
            EyeBlink = (EyeBlink)random.Next(0,(int)EyeBlink.Blink),

            MouthType = (MouthType)random.Next(0,(int)MouthType.None),
            MouthMode = (MouthMode)random.Next(0,(int)MouthMode.Talk)
        };
    }
}
