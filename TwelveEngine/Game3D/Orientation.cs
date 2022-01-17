using Microsoft.Xna.Framework;

namespace TwelveEngine.Game3D {
    public static class Orientation {
        public readonly static Vector3 CameraForward = new Vector3(0,1,0);
        public readonly static Vector3 CameraUp = new Vector3(0,0,1);

        public readonly static Vector3 WorldForward = new Vector3(0,0,-1);
        public readonly static Vector3 WorldUp = new Vector3(0,-1,0);
    }
}
