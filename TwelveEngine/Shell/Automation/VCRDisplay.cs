using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Shell.Automation {
    internal sealed class VCRDisplay {

        private const int RENDER_SIZE = 60;
        private const int SPACE = 4;

        private static readonly TimeSpan AdvanceFrameTimeout = TimeSpan.FromMilliseconds(250);

        private const int GLPYH_SIZE = 15;

        private enum Mode { None, Recording, Playback };

        private enum Symbol { Record, Play, Pause, Load, Advance, AdvanceMany };

        private readonly Dictionary<Symbol,Rectangle> symbolSources = new() {
            { Symbol.Record, GetSource(new Point(1,1)) },
            { Symbol.Play, GetSource(new Point(17,1)) },
            { Symbol.Pause, GetSource(new Point(1,17),new Point(-1,0)) },

            { Symbol.Load, GetSource(new Point(17,17)) },
            { Symbol.Advance, GetSource(new Point(33,1)) },
            { Symbol.AdvanceMany, GetSource(new Point(49,1)) }
        };

        private static Rectangle GetSource(Point source,Point? sizeOffset = null) {
            Point offset = sizeOffset ?? Point.Zero;
            Rectangle recetangle = new() {
                X = source.X,
                Y = source.Y,
                Width = GLPYH_SIZE + offset.X,
                Height = GLPYH_SIZE + offset.Y
            };
            return recetangle;
        }

        private static Mode GetMode() {
            if(AutomationAgent.PlaybackActive) {
                return Mode.Playback;
            }
            if(AutomationAgent.RecordingActive) {
                return Mode.Recording;
            }
            return Mode.None;
        }

        private GameManager Game { get; init; }
        private SpriteBatch spriteBatch;

        internal VCRDisplay(GameManager game) => Game = game;

        private Texture2D vcrTexture;

        public void Load() {
            vcrTexture = Game.Content.Load<Texture2D>("vcr");
            spriteBatch = Game.SpriteBatch;
        }

        private void DrawSymbol(ref int x,Symbol symbol) {
            var destination = new Rectangle(
                x,SPACE,RENDER_SIZE,RENDER_SIZE
            );
            var source = symbolSources[symbol];

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
            var mode = GetMode();

            int x = SPACE;

            bool paused = Game.IsPaused;
            bool loading = AutomationAgent.PlaybackLoading;

            bool hasMode = mode != Mode.None;

            bool hasFrameAdvance = lastFrameAdvance.HasValue;

            bool willDraw = hasMode || paused || loading || hasFrameAdvance;

            if(!willDraw) {
                return;
            }
            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.NonPremultiplied,SamplerState.PointClamp);

            if(hasMode) {
                if(mode == Mode.Playback) {
                    DrawSymbol(ref x,Symbol.Play);
                } else {
                    DrawSymbol(ref x,Symbol.Record);
                }
            }

            if(paused) {
                DrawSymbol(ref x,Symbol.Pause);
            }
            if(loading) {
                DrawSymbol(ref x,Symbol.Load);
            }

            if(lastFrameAdvance.HasValue) {
                var timeDifference = gameTime.TotalGameTime - lastFrameAdvance.Value;
                if(timeDifference < AdvanceFrameTimeout) {
                    DrawSymbol(ref x,smallAdvance ? Symbol.Advance : Symbol.AdvanceMany);
                }
            }

            spriteBatch.End();

        }
    }
}
