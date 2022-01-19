using TwelveEngine.Serial;

namespace TwelveEngine.Game3D {
    internal class CameraSerializer {

        private readonly World world;
        public CameraSerializer(World world) => this.world = world;

        private const byte NULL_CAM_ID = 0, ANGLE_CAM_ID = 1, TARGET_CAM_ID = 2, UNKNOWN_CAM_TYPE = 3;

        public void ExportCamera(SerialFrame frame) {
            var camera = world.Camera;
            if(camera == null) {
                frame.Set(NULL_CAM_ID);
            } else if(camera is AngleCamera angleCamera) {
                frame.Set(ANGLE_CAM_ID);
                frame.Set(angleCamera);
            } else if(camera is TargetCamera targetCamera) {
                frame.Set(TARGET_CAM_ID);
                frame.Set(targetCamera);
            } else {
                frame.Set(UNKNOWN_CAM_TYPE);
                frame.Set(camera);
            }
        }

        public void ImportCamera(SerialFrame frame) {
            var camType = frame.GetByte();
            var camera = world.Camera;
            switch(camType) {
                case NULL_CAM_ID:
                    world.Camera = null;
                    break;
                case ANGLE_CAM_ID:
                    if(camera is AngleCamera angleCamera) {
                        frame.Get(angleCamera);
                    } else {
                        var newCamera = new AngleCamera();
                        frame.Get(newCamera);
                        world.Camera = newCamera;
                    }
                    break;
                case TARGET_CAM_ID:
                    if(camera is TargetCamera targetCamera) {
                        frame.Get(targetCamera);
                    } else {
                        var newCamera = new TargetCamera();
                        frame.Get(newCamera);
                        world.Camera = newCamera;
                    }
                    break;
                default:
                case UNKNOWN_CAM_TYPE:
                    frame.Get(camera);
                    break;
            }
        }
    }
}
