using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TwelveEngine.Shell {

    internal readonly struct SpriteBatchSettingsFrame {
        public SpriteBatchSettingsFrame(SpriteBatchSettings settings) {
            BlendState = settings.BlendState;
            SamplerState = settings.SamplerState;
            DepthStencilState = settings.DepthStencilState;
            RasterizerState = settings.RasterizerState;
            Effect = settings.Effect;
            TransformMatrix = settings.TransformMatrix;
        }

        public void Apply(SpriteBatchSettings settings) {
            settings.BlendState = BlendState;
            settings.SamplerState = SamplerState;
            settings.DepthStencilState = DepthStencilState;
            settings.RasterizerState = RasterizerState;
            settings.Effect = Effect;
            settings.TransformMatrix = TransformMatrix;
        }

        public readonly BlendState BlendState;
        public readonly SamplerState SamplerState;
        public readonly DepthStencilState DepthStencilState;
        public readonly RasterizerState RasterizerState;
        public readonly Effect Effect;
        public readonly Matrix? TransformMatrix;
    }

    internal sealed class SpriteBatchSettings {

        public BlendState BlendState { get; set; }
        public SamplerState SamplerState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public RasterizerState RasterizerState { get; set; }
        public Effect Effect { get; set; }
        public Matrix? TransformMatrix { get; set; }

        private readonly Stack<SpriteBatchSettingsFrame> frameStack = new Stack<SpriteBatchSettingsFrame>();

        public void Clear() {
            BlendState = null;
            SamplerState = null;
            DepthStencilState = null;
            RasterizerState = null;
            Effect = null;
            TransformMatrix = null;
        }

        public void Save(bool clearSettings) {
            frameStack.Push(new SpriteBatchSettingsFrame(this));
            if(clearSettings) {
                Clear();
            }
        }
        
        public void Restore() {
            if(!frameStack.TryPop(out var frame)) {
                return;
            }
            frame.Apply(this);
        }
    }
}
