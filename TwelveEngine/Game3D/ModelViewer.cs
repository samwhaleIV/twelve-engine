﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.GameUI;
using TwelveEngine.Game3D.Entity.Types;
using TwelveEngine.Game3D.Entity;

namespace TwelveEngine.Game3D {

    public static class ModelViewer {
        public static ModelViewer<AnimatedModelEntity> CreateAnimated(string modelName,string textureName = null) {
            return new ModelViewer<AnimatedModelEntity>(modelName,textureName);
        }
        public static ModelViewer<ModelEntity> CreateStatic(string modelName,string textureName = null) {
            return new ModelViewer<ModelEntity>(modelName,textureName);
        }
    }

    public sealed class ModelViewer<TModelEntity>:World where TModelEntity : ModelBase, new() {

        private enum ControlMode { Camera, Model, Animation }
        private ControlMode controlMode = ControlMode.Camera;

        private const float VELOCITY_BASE = 1f / (1000f / 60f);
        public const float MOUSE_SPEED = 0.15f;

        public const float ANIMATION_RATE_CHANGE = 0.125f;

        public float ModelRotationSpeed { get; set; } = 2.5f;
        public float FreeCamSpeed { get; set; } = 0.05f;

        private ImpulseGuide ImpulseGuide { get; set; }

        private readonly string modelName;
        private readonly string textureName;

        private int ModelID = EntitySystem.EntityManager.START_ID - 1;

        internal ModelViewer(string modelName,string textureName) {
            this.modelName = modelName;
            this.textureName = textureName;

            OnLoad += ModelTest_OnLoad;
            OnUnload += ModelViewer_OnUnload;

            OnUpdate += ModelTest_OnUpdate;
            OnRender += ModelTest_OnRender;

            OnPreRender += PreRenderEntities;
        }

        private void UpdateImpulseGuide() {
            switch(controlMode) {
                case ControlMode.Camera:
                    ImpulseGuide.SetDescriptions(
                        (Impulse.Toggle, "Control Mode (Camera)"),
                        (Impulse.Up, "Forward"),
                        (Impulse.Down, "Back"),
                        (Impulse.Left, "Left"),
                        (Impulse.Right, "Right"),
                        (Impulse.Ascend, "Up"),
                        (Impulse.Descend, "Down")
                    );
                    break;
                case ControlMode.Model:
                    ImpulseGuide.SetDescriptions(
                        (Impulse.Toggle, "Control Mode (Model)"),
                        (Impulse.Up, "-Pitch"),
                        (Impulse.Down, "+Pitch"),
                        (Impulse.Left, "-Roll"),
                        (Impulse.Right, "+Roll"),
                        (Impulse.Ascend, "+Yaw"),
                        (Impulse.Descend, "-Yaw")
                    );
                    break;
                case ControlMode.Animation:
                    ImpulseGuide.SetDescriptions(
                        (Impulse.Toggle, "Control Mode (Animation)"),
                        (Impulse.Accept, "Toggle Play"),
                        (Impulse.Left, "Toggle Looping"),
                        (Impulse.Right, "Cycle Animation"),
                        (Impulse.Up, $"Speed (+{ANIMATION_RATE_CHANGE}x)"),
                        (Impulse.Down, $"Speed (-{ANIMATION_RATE_CHANGE}x)")
                    );
                    break;
            }
        }

        private SerialAnimationPlayer GetAnimationPlayer() {
            var entity = Entities.Get<AnimatedModelEntity>(ModelID);
            var animationPlayer = entity.AnimationPlayer;
            return animationPlayer;
        }

        private void Input_OnDirectionDown(Direction direction) {
            var animationPlayer = GetAnimationPlayer();
            switch(direction) {
                case Direction.Up:
                    animationPlayer.PlaybackSpeed += ANIMATION_RATE_CHANGE;
                    break;
                case Direction.Down:
                    animationPlayer.PlaybackSpeed -= ANIMATION_RATE_CHANGE;
                    break;
                case Direction.Left:
                    animationPlayer.IsLooping = !animationPlayer.IsLooping;
                    break;
                case Direction.Right:
                    animationPlayer.AnimationIndex++;
                    break;
            }
        }

        private void ModelViewer_OnUnload() {
            Input.OnToggleDown -= Input_OnToggleDown;
            if(controlMode == ControlMode.Animation) {
                UnbindAnimationController();
            }
            Game.OnWriteDebug -= Game_OnWriteDebug;
        }

        private void BindAnimationController() {
            Input.OnDirectionDown += Input_OnDirectionDown;
            Input.OnAcceptDown += Input_OnAcceptDown;
        }

        private void UnbindAnimationController() {
            Input.OnDirectionDown -= Input_OnDirectionDown;
            Input.OnAcceptDown -= Input_OnAcceptDown;
        }

        private void Input_OnAcceptDown() {
            var animationPlayer = GetAnimationPlayer();
            animationPlayer.IsPlaying = !animationPlayer.IsPlaying;
        }

        private void Input_OnToggleDown() {
            switch(controlMode) {
                case ControlMode.Camera:
                    controlMode = ControlMode.Model;
                    break;
                case ControlMode.Model:
                    if(Entities.Get(ModelID) is AnimatedModelEntity) {
                        controlMode = ControlMode.Animation;
                        BindAnimationController();
                    } else {
                        controlMode = ControlMode.Camera;
                    }
                    break;
                case ControlMode.Animation:
                    UnbindAnimationController();
                    controlMode = ControlMode.Camera;
                    break;

            }
            UpdateImpulseGuide();
        }

        private void ModelTest_OnLoad() {
            var camera = new AngleCamera() {
                NearPlane = 0.1f,
                FieldOfView = 75f,
                Angle = new Vector2(219.75f,215.10f),
                Position = new Vector3(1.24f,-1.4f,1.34f)
            };
            Camera = camera;

            Entities.Create(Entity3DType.GridLines);

            ModelID = Entities.Add(new TModelEntity() {
                Texture = textureName,
                Model = modelName
            }).ID;

            Input.OnToggleDown += Input_OnToggleDown;
            ImpulseGuide = new ImpulseGuide(Game);

            UpdateImpulseGuide();

            Game.OnWriteDebug += Game_OnWriteDebug;
        }

        private float GetVelocity(GameTime gameTime,float velocity) {
            return (float)gameTime.ElapsedGameTime.TotalMilliseconds * VELOCITY_BASE * velocity;
        }

        private float GetRotationVelocity(GameTime gameTime) => GetVelocity(gameTime,ModelRotationSpeed);
        private float GetFreeCamVelocity(GameTime gameTime) => GetVelocity(gameTime,FreeCamSpeed);

        private WorldMatrixEntity GetMatrixEntity() {
            return Entities.Get<WorldMatrixEntity>(ModelID);
        }

        private void RotateModel(GameTime gameTime) {
            var matrixEntity = GetMatrixEntity();
            if(matrixEntity == null) {
                return;
            }

            var rotationDelta = Input.GetDelta3D();
            if(rotationDelta == Vector3.Zero) {
                return;
            }
            float velocity = GetRotationVelocity(gameTime);

            matrixEntity.Rotation += rotationDelta * new Vector3(velocity);
        }

        private void UpdateCamera(GameTime gameTime) {
            if(!(Camera is AngleCamera angleCamera)) {
                return;
            }
            var mouseDelta = Game.MouseHandler.Delta;
            if(Game.MouseHandler.Capturing && mouseDelta != Point.Zero) {
                mouseDelta.Y = -mouseDelta.Y;
                angleCamera.AddAngle(mouseDelta.ToVector2() * MOUSE_SPEED);
            }
            angleCamera.UpdateFreeCam(Input.GetDelta3D(),GetFreeCamVelocity(gameTime));
        }

        private void ModelTest_OnUpdate(GameTime gameTime) {
            if(controlMode == ControlMode.Camera) {
                UpdateCamera(gameTime);
            }
            Camera.Update(Game.Viewport.AspectRatio);
            if(controlMode == ControlMode.Model) {
                RotateModel(gameTime);
            }
        }

        private static void ResetGraphicsDeviceState(GraphicsDevice graphicsDevice) {
            graphicsDevice.Clear(Color.Gray);
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        }

        private void DrawDefaultDebug(DebugWriter writer) {
            writer.Write(Camera.Position);
            if(!(Camera is AngleCamera angleCamera)) {
                return;
            }
            writer.WriteXY(angleCamera.Yaw,angleCamera.Pitch,"Yaw","Pitch");
        }

        private void Game_OnWriteDebug(DebugWriter writer) {
            writer.ToTopLeft();
            if(controlMode == ControlMode.Animation) {
                GetAnimationPlayer().WriteDebug(writer);
            } else {
                DrawDefaultDebug(writer);
            }
        }

        private void ModelTest_OnRender(GameTime gameTime) {
            ResetGraphicsDeviceState(Game.GraphicsDevice);
            RenderEntities(gameTime);
            Game.SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            ImpulseGuide.Render();
            Game.SpriteBatch.End();
        }

    }
}
