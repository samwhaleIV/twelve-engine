using Elves.Battle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine;
using TwelveEngine.Shell;

namespace Elves.Scenes.Battle.UI {
    public sealed class MiniGameScreen {

        public MiniGame MiniGame { get; set; }

        public int Width { get; init; }
        public int Height { get; init; }

        public Texture2D Texture { get; set; }

        public Vector2 SourceOffset { get; private init; }  = Constants.BattleUI.MiniGameScreenInnerArea.Location.ToVector2();

        public RenderTarget2D Buffer { get; private set; }

        private readonly Interpolator offscreenAnimator = new(Constants.BattleUI.MiniGameMovement);

        public bool _showing = false;

        public void Show(TimeSpan now) {
            offscreenAnimator.ResetCarryOver(now);
            _showing = true;
            MiniGame?.Activate();
        }

        public void Hide(TimeSpan now) {
            offscreenAnimator.ResetCarryOver(now);
            _showing = false;
            MiniGame?.Deactivate();
        }

        public void Load(GraphicsDevice graphicsDevice) {
            Buffer = new(graphicsDevice,Width,Height,false,SurfaceFormat.Color,DepthFormat.None,0,RenderTargetUsage.PreserveContents);
        }

        public bool ExternalInputDisabled => _showing || !offscreenAnimator.IsFinished;

        public void Unload() {
            Buffer?.Dispose();
            Buffer = null;
        }

        private FloatRectangle _area, _overlayArea;

        public void UpdateLayout(TimeSpan now,FloatRectangle viewport,float scale) {
            offscreenAnimator.Update(now);
            var size = new Vector2(Width,Height) * scale;
            FloatRectangle area = new(viewport.Center-size*0.5f,size);

            float startY = area.Y, endY = viewport.Bottom;
            if(_showing) (startY, endY) = (endY, startY);

            area.Y = offscreenAnimator.SmoothStep(startY,endY);
            _area = area;

            Vector2 overlaySize = Texture.Bounds.Size.ToVector2() * scale;
            Vector2 offset = SourceOffset * scale;

            _overlayArea = new(area.Position - offset,overlaySize);
        }

        private bool ShouldRender => MiniGame is not null && _showing || !offscreenAnimator.IsFinished;

        public void PreRender(GraphicsDevice graphicsDevice,SpriteBatch spriteBatch,RenderTargetStack renderTargetStack) {
            if(!ShouldRender) {
                return;
            }
            renderTargetStack.Push(Buffer);
            graphicsDevice.Clear(MiniGame.ClearColor);
            MiniGame.Render(spriteBatch,Width,Height);
            renderTargetStack.Pop();
        }

        public void Render(SpriteBatch spriteBatch) {
            if(!ShouldRender) {
                return;
            }
            spriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);
            spriteBatch.Draw(Buffer,_area.ToRectangle(),Color.White);
            spriteBatch.Draw(Texture,_overlayArea.ToRectangle(),Color.White);
            spriteBatch.End();
        }
    }
}
