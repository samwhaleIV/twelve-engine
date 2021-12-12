using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Automation {
    internal sealed class VCRDisplay {

        private const int RENDER_SIZE = 60;
        private const int SPACE = 4;
        private const double ADVANCE_FRAME_TIMEOUT = 250;

        private bool Loading => automationAgent.PlaybackLoading;
        private bool Paused => gameManager.Paused;

        private enum Mode {
            None,
            Recording,
            Playback
        };

        private enum Symbol {
            Record, Play, Pause, Load, Advance, AdvanceMany
        };

        private (int x, int y)[] symbolSources = {
            (1,1),(17,1),(1,17),(17,17),(33,1),(49,1)
        };

        private Rectangle getSymbolSource(Symbol symbol) {
            var (x,y) = symbolSources[(int)symbol];
            var source = new Rectangle();
            source.X = x;
            source.Y = y;
            source.Width = 15;
            source.Height = 15;
            return source;
        }

        private Mode getMode() {
            var automationAgent = gameManager.AutomationAgent;
            if(automationAgent.PlaybackActive) {
                return Mode.Playback;
            }
            if(automationAgent.RecordingActive) {
                return Mode.Recording;
            }
            return Mode.None;
        }

        private readonly AutomationAgent automationAgent;
        private readonly GameManager gameManager;
        private SpriteBatch spriteBatch;

        internal VCRDisplay(GameManager gameManager,AutomationAgent automationAgent) {
            this.automationAgent = automationAgent;
            this.gameManager = gameManager;
        }

        private Texture2D vcrTexture;

        public void Load() {
            vcrTexture = gameManager.Content.Load<Texture2D>("vcr");
            spriteBatch = gameManager.SpriteBatch;
        }

        private void drawSymbol(ref int x,Symbol symbol) {
            var destination = new Rectangle(
                x,SPACE,RENDER_SIZE,RENDER_SIZE
            );
            var source = getSymbolSource(symbol);

            spriteBatch.Draw(vcrTexture,destination,source,Color.White);

            x += RENDER_SIZE + SPACE;
        }

        private TimeSpan? lastFrameAdvance;
        private bool smallAdvance = true;

        public void AdvanceFrame(GameTime gameTime) {
            smallAdvance = true;
            lastFrameAdvance = gameTime.TotalGameTime;
        }
        public void AdvanceFramesMany(GameTime gameTime) {
            smallAdvance = false;
            lastFrameAdvance = gameTime.TotalGameTime;
        }

        internal void Render(GameTime gameTime) {
            var mode = getMode();

            int x = SPACE;

            bool paused = Paused;
            bool loading = Loading;

            bool hasMode = mode != Mode.None;

            bool hasFrameAdvance = lastFrameAdvance.HasValue;

            bool willDraw = hasMode || paused || loading || hasFrameAdvance;

            if(!willDraw) {
                return;
            }
            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.NonPremultiplied,SamplerState.PointClamp);

            if(hasMode) {
                if(mode == Mode.Playback) {
                    drawSymbol(ref x,Symbol.Play);
                } else {
                    drawSymbol(ref x,Symbol.Record);
                }
            }

            if(paused) {
                drawSymbol(ref x,Symbol.Pause);
            }
            if(loading) {
                drawSymbol(ref x,Symbol.Load);
            }

            if(lastFrameAdvance.HasValue) {
                var timeDifference = gameTime.TotalGameTime - lastFrameAdvance.Value;
                if(timeDifference.TotalMilliseconds < ADVANCE_FRAME_TIMEOUT) {
                    drawSymbol(ref x,smallAdvance ? Symbol.Advance : Symbol.AdvanceMany);
                }
            }

            spriteBatch.End();

        }
    }
}
