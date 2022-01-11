using System;
using TwelveEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ElfGame.ElfSprite.Face;

namespace ElfGame {
    internal class FacialAnimationViewer:GameState {

        private readonly ElfFace elfFace = new ElfFace();
        private readonly Random random = new Random();

        private RenderTarget2D renderBuffer;

        public FacialAnimationViewer() {
            OnLoad += FacialAnimationViewer_OnLoad;
            OnRender += FacialAnimationViewer_OnRender;
            OnPreRender += FacialAnimationViewer_OnPreRender;
        }

        private void FacialAnimationViewer_OnLoad() {
            Input.OnAcceptDown += Input_OnAcceptDown;
            Input.OnDirectionDown +=Input_OnDirectionDown;
            OnUnload += FacialAnimationViewer_OnUnload;

            elfFace.Renderer.Texture = Game.Content.Load<Texture2D>(Constants.ElfFaceImage);

            var size = elfFace.Renderer.Size;
            renderBuffer = new RenderTarget2D(Game.GraphicsDevice,size.X,size.Y);

            elfFace.GameState = this;
        }

        private void Input_OnDirectionDown(Direction direction) {
            switch(direction) {
                case Direction.Left:
                    elfFace.Eyes.LeftWink();
                    break;
                case Direction.Right:
                    elfFace.Eyes.RightWink();
                    break;
                case Direction.Up:
                    elfFace.Eyes.StartBlinking();
                    break;
                case Direction.Down:
                    elfFace.Eyes.StopBlinking();
                    break;
            }
        }

        private void FacialAnimationViewer_OnUnload() {
            Input.OnAcceptDown -= Input_OnAcceptDown;
            Input.OnDirectionDown -= Input_OnDirectionDown;
            renderBuffer?.Dispose();
        }

        private void Input_OnAcceptDown() {
            elfFace.State = FaceState.GetRandom(random);
        }

        private void FacialAnimationViewer_OnPreRender(GameTime gameTime) {
            Game.GraphicsDevice.SetRenderTarget(renderBuffer);

            Game.SpriteBatch.Begin(SpriteSortMode.Deferred);
            elfFace.Renderer.Render(Game.SpriteBatch,Point.Zero);
            Game.SpriteBatch.End();

            Game.GraphicsDevice.SetRenderTarget(null);
        }

        private void FacialAnimationViewer_OnRender(GameTime gameTime) {
            Game.SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);

            var bounds = Game.GraphicsDevice.Viewport.Bounds;
            var size = new Vector2(Math.Min(bounds.Width,bounds.Height)) * 0.9f;
            var location = bounds.Center.ToVector2() - size * 0.5f;

            Game.SpriteBatch.Draw(renderBuffer,new Rectangle(location.ToPoint(),size.ToPoint()),Color.White);

            Game.SpriteBatch.End();
        }
    }
}
