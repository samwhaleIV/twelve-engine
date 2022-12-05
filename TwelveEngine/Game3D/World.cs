using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game3D.Entity;
using TwelveEngine.Shell.States;

namespace TwelveEngine.Game3D {
    public class World:InputGameState {

        public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

        public World() {
            OnLoad += World_OnLoad;
            OnUpdate += World_OnUpdate;
            OnRender += World_OnRender;
        }

        public EntityManager<Entity3D,World> Entities { get; private set; }

        private void World_OnLoad() {
            Entities = new EntityManager<Entity3D,World>(this);
            if(pendingSamplerState == null) {
                return;
            }
            Game.GraphicsDevice.SamplerStates[0] = pendingSamplerState;
            pendingSamplerState = null;
        }

        public Color ClearColor { get; set; } = Color.Black;

        private void World_OnRender(GameTime gameTime) {
            Game.GraphicsDevice.Clear(ClearColor);
        }

        private readonly Stack<SamplerState> samplerStateStack = new Stack<SamplerState>();

        private SamplerState pendingSamplerState = SamplerState.LinearClamp;
        public SamplerState SamplerState {
            get => Game?.GraphicsDevice.SamplerStates[0] ?? pendingSamplerState;
            set {
                if(Game == null) {
                    pendingSamplerState = value;
                } else {
                    samplerStateStack.Clear();
                    Game.GraphicsDevice.SamplerStates[0] = value;
                }
            }
        }

        public void PushSamplerState(SamplerState samplerState) {
            samplerStateStack.Push(SamplerState);
            if(Game == null) {
                pendingSamplerState = samplerState;
            } else {
                Game.GraphicsDevice.SamplerStates[0] = samplerState;
            }
        }

        public void PopSamplerState() {
            if(!samplerStateStack.TryPop(out SamplerState samplerState)) {
                return;
            }
            if(Game == null) {
                pendingSamplerState = samplerState;
            } else {
                Game.GraphicsDevice.SamplerStates[0] = samplerState;
            }
        }

        private void World_OnUpdate(GameTime gameTime) {
            UpdateInputs(gameTime);
            _camera?.Update(AspectRatio); /* An entity might need to use orthographic projection information */
            Entities.Iterate(Entity3D.Update,gameTime);
            _camera?.Update(AspectRatio);
        }

        public void RenderEntities(GameTime gameTime) {
            Entities.Iterate(Entity3D.Render,gameTime);
        }

        public void PreRenderEntities(GameTime gameTime) {
            Entities.Iterate(Entity3D.PreRender,gameTime);
        }

        private Camera3D _camera;
        public Camera3D Camera { get => _camera; set => SetNewCamera(value); }

        public Matrix ViewMatrix => _camera?.ViewMatrix ?? Matrix.Identity;
        public Matrix ProjectionMatrix => _camera?.ProjectionMatrix ?? Matrix.Identity;

        public event Action<Matrix> OnViewMatrixChanged, OnProjectionMatrixChanged;

        private void FireProjectionMatrixChanged(Matrix projectionMatrix) {
            OnProjectionMatrixChanged?.Invoke(projectionMatrix);
        }
        private void FireViewMatrixChanged(Matrix viewMatrix) {
            OnViewMatrixChanged?.Invoke(viewMatrix);
        }

        public float AspectRatio => Game.Viewport.AspectRatio;

        private void SetNewCamera(Camera3D newCamera) {
            var oldCamera = _camera;
            if(newCamera == oldCamera) {
                return;
            }
            if(oldCamera != null) {
                oldCamera.OnProjectionMatrixChanged -= FireProjectionMatrixChanged;
                oldCamera.OnViewMatrixChanged -= FireViewMatrixChanged;
            }
            _camera = newCamera;
            if(newCamera == null) {
                FireProjectionMatrixChanged(Matrix.Identity);
                FireViewMatrixChanged(Matrix.Identity);
                return;
            }

            newCamera.OnProjectionMatrixChanged += FireProjectionMatrixChanged;
            newCamera.OnViewMatrixChanged += FireViewMatrixChanged;

            FireProjectionMatrixChanged(newCamera.ProjectionMatrix);
            FireViewMatrixChanged(newCamera.ViewMatrix);
        }

        public BufferSet CreateBufferSet<TVertices>(TVertices[] vertices) where TVertices:struct {
            return BufferSet.Create(GraphicsDevice,vertices);
        }
    }
}
