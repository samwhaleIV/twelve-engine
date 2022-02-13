using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TwelveEngine.Shell {

    public sealed class SmartSpriteBatch:SpriteBatch {

        private readonly GameManager game;
        internal SmartSpriteBatch(GameManager game) : base(game.GraphicsDevice) => this.game = game;

        public bool Active { get; private set; }

        private SpriteBatchSettings Settings => game.SpriteBatchSettings;

        public new void Begin(
            SpriteSortMode sortMode = SpriteSortMode.Deferred,
            BlendState blendState = null,
            SamplerState samplerState = null,
            DepthStencilState depthStencilState = null,
            RasterizerState rasterizerState = null,
            Effect effect = null,
            Matrix? transformMatrix = null
        ) {
            if(Active) {
                throw new InvalidOperationException("Cannot begin a new batch before the previous one has ended");
            }

            var settings = Settings;
            base.Begin(
                sortMode,
                blendState ?? settings.BlendState,
                samplerState ?? settings.SamplerState,
                depthStencilState ?? settings.DepthStencilState,
                rasterizerState ?? settings.RasterizerState,
                effect ?? settings.Effect,
                transformMatrix ?? settings.TransformMatrix
            );

            Active = true;
        }

        public bool TryBegin(SpriteSortMode spriteSortMode = SpriteSortMode.Deferred) {
            if(Active) return false;

            var settings = Settings;
            Begin(
                spriteSortMode,
                settings.BlendState,
                settings.SamplerState,
                settings.DepthStencilState,
                settings.RasterizerState,
                settings.Effect,
                settings.TransformMatrix
            );

            Active = true;
            return true;
        }

        public new void End() {
            if(!Active) {
                throw new InvalidOperationException("Cannot end a batch when a one has not been started");
            }
            base.End();
            Active = false;
        }

        public bool TryEnd() {
            if(!Active) {
                return false;
            }
            base.End();
            return true;
        }

        public void SaveSettings(bool clearSettings = true) => Settings.Save(clearSettings);
        public void RestoreSettings() => Settings.Restore();
        public void ClearSettings() => Settings.Clear();

        public BlendState BlendState {
            get => Settings.BlendState;
            set => Settings.BlendState = value;
        }

        public SamplerState SamplerState {
            set => Settings.SamplerState = value;
            get => Settings.SamplerState;
        }

        public DepthStencilState DepthStencilState {
            set => Settings.DepthStencilState = value;
            get => Settings.DepthStencilState;
        }

        public RasterizerState RasterizerState {
            set => Settings.RasterizerState = value;
            get => Settings.RasterizerState;
        }

        public Effect Effect {
            set => Settings.Effect = value;
            get => Settings.Effect;
        }

        public Matrix? TransformMatrix {
            set => Settings.TransformMatrix = value;
            get => Settings.TransformMatrix;
        }
    }
}
