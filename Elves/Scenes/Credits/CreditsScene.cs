using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TwelveEngine;
using TwelveEngine.Font;
using TwelveEngine.Shell;

namespace Elves.Scenes.Credits {

    public class CreditsScene:InputGameState {

        private static readonly Color HardColor = Color.White, SoftColor = Color.LightGray, LinkColor = new(0,132,238);

        private readonly List<(Texture2D Texture,float Y,Vector2 ScaledSize,Vector2 Scale)> _imageItems = new();
        private readonly List<(CreditsItem TextItem,float Y)> _textItems = new();

        private readonly TimeSpan _duration;

        private readonly UVSpriteFont _font;
        private readonly float _lineHeight, _endY, _scale;

        public float Progress => MathF.Max(MathF.Min((float)(LocalRealTime / _duration),1),0);

        private readonly Queue<(FloatRectangle Area, string URL)> _urlBuffer = new();

        private float CategorizeItems(CreditsItem[] items) {

            float y = 0, rowPadding = _lineHeight;

            foreach(var item in items) {
                switch(item.Type) {
                    case CreditsItemType.LineBreak:
                        y += _font.LineHeight;
                        break;
                    case CreditsItemType.TextLabel:
                    case CreditsItemType.TextHeading:
                    case CreditsItemType.Text:
                    case CreditsItemType.Link:
                        _textItems.Add((item,y));
                        y += _font.LineHeight;
                        break;
                    case CreditsItemType.Image:
                        var texture = item.Texture;
                        var scale = new Vector2(item.Scale);
                        var size = new Vector2(texture.Width,texture.Height) * item.Scale;
                        _imageItems.Add((texture,y,size,scale));
                        y += size.Y / _scale;
                        break;
                }
                y += rowPadding;
            }
            return y - rowPadding;
        }

        public CreditsScene(UVSpriteFont font,float textScale,CreditsItem[] items) {
            _font = font;
            _lineHeight = _font.LineHeight;
            _scale = textScale;
            _endY = CategorizeItems(items);
            _duration = _endY / _lineHeight * Constants.AnimationTiming.CreditsRowDuration;
            OnRender.Add(RenderCredits);
            OnUpdate.Add(UpdateURLInteraction,EventPriority.Second);
            Impulse.Router.OnAcceptDown += EndScene;
            Impulse.Router.OnCancelDown += EndScene;
        }

        private void EndScene() {
            if(Progress < 1) {
                return;
            }
            throw new NotImplementedException();
        }

        private bool _mousePressed = false;

        private void UpdateURLInteraction() {
            string selectedURL = null;
            foreach(var item in _urlBuffer) {
                if(!item.Area.Contains(Mouse.Position)) {
                    continue;
                }
                selectedURL = item.URL;
                break;
            }
            bool mousePressed = Mouse.CapturingLeft;
            bool wasPressingLastFrame = _mousePressed;
            _mousePressed = mousePressed;
            CustomCursor.State = selectedURL is null ? CursorState.Default : CursorState.Interact;
            if(!mousePressed || (mousePressed && wasPressingLastFrame) || selectedURL is null) {
                return;
            }
            Program.TryOpenURL(selectedURL);
        }

        private void RenderCredits() {
            FloatRectangle viewport = new(Viewport);
            float xCenter = MathF.Floor(viewport.CenterX);

            float lineHeight = _scale * _lineHeight;
            float halfLineHeight = lineHeight * 0.5f;

            float progress = Progress;
            float yOffset = -(progress * _endY * _scale - Viewport.Height * (1 - progress)); /* Screen area translation... */

            float labelLeftX = xCenter - halfLineHeight, labelRightX = xCenter + halfLineHeight;

            bool InRange(float y,float height) => viewport.Top < y + height && y < viewport.Bottom;
            float GetY(float itemY) => itemY * _scale + yOffset;

            SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            foreach(var imageItem in _imageItems) {
                float y = GetY(imageItem.Y);
                Texture2D texture = imageItem.Texture;

                Vector2 size = imageItem.ScaledSize;
                if(!InRange(y,size.Y)) {
                    continue;
                }
                Vector2 position = new(xCenter,y);
                position.X -= size.X * 0.5f;
                SpriteBatch.Draw(texture,position,null,Color.White,0f,Vector2.Zero,imageItem.Scale,SpriteEffects.None,1f);
            }
            SpriteBatch.End();

            _urlBuffer.Clear();

            _font.Begin(SpriteBatch);
            foreach(var (TextItem, Y) in _textItems) {
                float y = GetY(Y);
                if(!InRange(y,lineHeight)) {
                    continue;
                }
                if(TextItem.IsLabel) {
                    _font.DrawRight(TextItem.Label,new(labelLeftX,y),_scale,HardColor);
                    _font.Draw(TextItem.TextValue,new(labelRightX,y),_scale,SoftColor);             
                    continue;
                }
                if(TextItem.IsHeading) {
                    _font.DrawCentered(TextItem.TextValue,new(xCenter,y+halfLineHeight),_scale,HardColor);
                    continue;
                }
                Color color = SoftColor;
                if(TextItem.IsLink) {
                    color = LinkColor;
                }
                
                FloatRectangle area = _font.DrawCentered(TextItem.TextValue,new(xCenter,y+halfLineHeight),_scale,color);
                if(!TextItem.IsLink) {
                    continue;
                }
                _urlBuffer.Enqueue((area, TextItem.TextValue));
                if(!area.Contains(Mouse.Position)) {
                    continue;
                }
                _font.DrawCenteredUnderline(area,_scale,color);
            }
            _font.End();
        }
    }
}
 