using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game3D.Entity;
using TwelveEngine.Shell;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Game3D {
    public class World:InputGameState {

        public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

        public World() {
            OnLoad += World_OnLoad;
            OnUpdate += UpdateGame;
            OnRender += World_OnRender;
            OnWriteDebug += World_OnWriteDebug;
        }

        public bool WriteDebugEnabled { get; set; } = false;

        private void World_OnWriteDebug(DebugWriter writer) {
            if(!WriteDebugEnabled) {
                return;
            }
            writer.ToTopLeft();
            writer.Write(Camera.Position);
            if(Camera is not AngleCamera angleCamera) {
                return;
            }
            writer.WriteXY(angleCamera.Yaw,angleCamera.Pitch,"Yaw","Pitch");
        }

        public EntityManager<Entity3D,World> Entities { get; private set; }

        private void World_OnLoad() {
            Entities = new EntityManager<Entity3D,World>(this);
        }

        public Color ClearColor { get; set; } = Color.Black;

        private void World_OnRender() {
            Game.GraphicsDevice.Clear(ClearColor);
        }

        protected void UpdateCamera() {
            Camera?.Update(AspectRatio);
        }

        protected virtual void UpdateGame() {
            UpdateInputs();
            UpdateCamera(); /* An entity might need to use orthographic projection information */
            Entities.Update();
            UpdateCamera();
        }

        public void RenderEntities() {
            Entities.Render();
        }

        public void PreRenderEntities() {
            Entities.PreRender();
        }

        public Camera3D Camera { get; set; } = null;

        public Matrix ViewMatrix => Camera?.ViewMatrix ?? Matrix.Identity;
        public Matrix ProjectionMatrix => Camera?.ProjectionMatrix ?? Matrix.Identity;

        public float AspectRatio => Game.Viewport.AspectRatio;

        public BufferSet CreateBufferSet<TVertices>(TVertices[] vertices) where TVertices:struct {
            return BufferSet.Create(GraphicsDevice,vertices);
        }
    }
}
