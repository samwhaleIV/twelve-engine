using System;
using TwelveEngine.Serial;
using Microsoft.Xna.Framework;
using TwelveEngine.Shell.States;

namespace TwelveEngine.Game3D {
    public sealed class AngleCamera:Camera3D {

        public const float PitchOffset = 180f;

        public const float MaxPitch = PitchOffset + 90f;
        public const float MinPitch = PitchOffset - 90f;

        public AngleCamera() => SetAngle(0f,PitchOffset);

        public Vector3 Up { get; private set; }
        public Vector3 Forward { get; private set; }

        public float Yaw { get; private set; }
        public float Pitch { get; private set; }

        private bool limitPitch = true;
        public bool LimitedPitch {
            get => limitPitch;
            set {
                if(value == limitPitch) {
                    return;
                }
                limitPitch = value;
                if(!limitPitch) {
                    return;
                }
                var newPitch = ClampPitch(Pitch);
                if(newPitch == Pitch) {
                    return;
                }
                SetAngle(Yaw,newPitch);
            }
        }

        public Vector2 Angle {
            get => new Vector2(Yaw,Pitch);
            set => SetAngle(value.X,value.Y);
        }

        private float NormalizeAngle(float degrees) => (degrees % 360 + 360) % 360;

        private float ClampPitch(float pitch) {
            return limitPitch ? Math.Clamp(pitch,MinPitch,MaxPitch) : pitch;
        }

        public void SetAngle(float yaw,float pitch) {
            Yaw = NormalizeAngle(yaw);
            Pitch = ClampPitch(NormalizeAngle(pitch));

            yaw = MathHelper.ToRadians(Yaw);
            pitch = MathHelper.ToRadians(Pitch + PitchOffset);

            Forward = Vector3.Transform(Vector3.Forward,Matrix.CreateFromAxisAngle(Vector3.Up,yaw));
            Forward.Normalize();

            Vector3 left = Vector3.Cross(Vector3.Up,Forward);
            left.Normalize();

            var angleMatrix = Matrix.CreateFromAxisAngle(left,pitch);
            Forward = Vector3.Transform(Forward,angleMatrix);
            Up = Vector3.Transform(Vector3.Up,angleMatrix);

            InvalidateViewMatrix();
        }

        public void AddAngle(float yaw,float pitch) {
            SetAngle(Yaw+yaw,Pitch+pitch);
        }
        public void AddAngle(Vector2 angle) {
            angle += Angle;
            SetAngle(angle.X,angle.Y);
        }
        public void AddPitch(float degrees) {
            AddAngle(0f,degrees);
        }
        public void AddYaw(float degrees) {
            AddAngle(degrees,0f);
        }

        protected override Matrix GetViewMatrix() {
            return Matrix.CreateLookAt(Position,Forward+Position,Up);
        }

        public void MoveLeftRight(float distance) {
            var left = Vector3.Cross(Up,Forward);
            left.Normalize();
            Position -= left * distance;
        }

        public void MoveFrontBack(float distance) {
            var forward = Forward;
            forward.Normalize();
            Position -= forward * distance;
        }

        public void MoveUpDown(float distance) {
            var position = Position;
            position.Y -= distance;
            Position = position;
        }

        public void MoveAngleRelative(Vector3 velocity) {
            MoveLeftRight(velocity.X);
            MoveFrontBack(velocity.Y);
            MoveUpDown(velocity.Z);
        }

        public void UpdateFreeCam(InputGameState gameState,float lookSpeed,float moveSpeed) {
            var mouseDelta = gameState.Mouse.Delta;
            if(gameState.Mouse.Capturing && mouseDelta != Point.Zero) {
                mouseDelta.Y = -mouseDelta.Y;
                AddAngle(mouseDelta.ToVector2() * lookSpeed);
            }

            var delta = gameState.Input.GetDelta3D();
            if(delta.X != 0) MoveLeftRight(delta.X * moveSpeed);
            if(delta.Y != 0) MoveUpDown(delta.Y * moveSpeed);
            if(delta.Z != 0) MoveFrontBack(delta.Z * moveSpeed);
        }

        public override void Export(SerialFrame frame) {
            base.Export(frame);
            frame.Set(Angle);
            frame.Set(LimitedPitch);
        }

        public override void Import(SerialFrame frame) {
            base.Import(frame);
            Angle = frame.GetVector2();
            LimitedPitch = frame.GetBool();
        }
    }
}
