using Microsoft.Xna.Framework;

namespace TwelveEngine.Game3D {
    public static class Orientation {
        public readonly static Vector3 Forward = new Vector3(0,1,0);
        public readonly static Vector3 Up = new Vector3(0,0,1);

        public const float CamPitchOffset = 180f;
        public const float CamYawOffset = 180f;

        public const float CamMaxPitch = CamPitchOffset + 90f;
        public const float CamMinPitch = CamPitchOffset - 90f;

        public const float WorldYawOffset = -180f;
        public const float WorldPitchOffset = 180f;
    }
}
