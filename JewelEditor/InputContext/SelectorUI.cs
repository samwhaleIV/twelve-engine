using TwelveEngine.UI;
using TwelveEngine.UI.Elements;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace JewelEditor.InputContext {
    internal sealed class SelectorUI {

        private UIEntity owner;
        public SelectorUI(UIEntity owner) => this.owner = owner;

        private static (Rectangle Area,InputMode InputMode)[] GetButtonSources() => new (Rectangle, InputMode)[] {
            (new Rectangle(16,16,16,16),InputMode.Pointer),
            (new Rectangle(16,32,16,16),InputMode.Entity),
            (new Rectangle(16,0,16,16),InputMode.NoTile),
            (new Rectangle(32,0,16,16),InputMode.FloorTile),
            (new Rectangle(48,0,16,16),InputMode.WallTile),
            (new Rectangle(0,16,16,16),InputMode.DoorTile)
        };

        private Dictionary<InputMode,ContextButton> buttonDictionary;

        private sealed class ContextButton:ImageButton {

            private readonly InputMode _inputMode;
            private readonly UIEntity owner;

            private Texture2D texture;

            public ContextButton(UIEntity owner,(Rectangle Area,InputMode InputMode) source):base(Editor.Tileset,source.Area) {
                this.owner = owner;
                _inputMode = source.InputMode;
                OnClick += ContextButton_OnClick;
                OnRender += ContextButton_OnRender;
                OnLoad += ContextButton_OnLoad;
            }

            private void ContextButton_OnLoad() {
                texture = GetColoredTexture(Color.Black);
            }

            private void ContextButton_OnRender(GameTime gameTime) {
                if(owner.InputMode != InputMode) {
                    return;
                }
                var lineArea = ScreenArea;
                lineArea.Height = 6;
                lineArea.Width += 4;
                lineArea.X -= 2;
                lineArea.Y = ScreenArea.Bottom + 4;
                Game.SpriteBatch.Draw(texture,lineArea,null,Color.White,0f,Vector2.Zero,SpriteEffects.None,Depth);
            }

            internal InputMode InputMode => _inputMode;

            private void ContextButton_OnClick(ImageButton self) {
                owner.InputMode = (self as ContextButton).InputMode;
            }
        }

        public void Generate(int panelHeight) {
            var panel = new Panel(Color.FromNonPremultiplied(255,255,255,127)) {
                Sizing = Sizing.PercentX,
                Width = 100,
                Height = panelHeight
            };

            var buttonSources = GetButtonSources();

            var buttonGroup = new RenderElement() {
                Sizing = Sizing.Normal,
                Height = panel.Height,
                Width = buttonSources.Length * panelHeight,
                Positioning = Positioning.CenterParentX
            };

            buttonDictionary = new Dictionary<InputMode,ContextButton>();

            for(int i = 0;i<buttonSources.Length;i++) {
                var parent = new RenderElement() {
                    Width = panelHeight,
                    Height = panelHeight,
                    X = i * panelHeight,
                    Padding = 4
                };
                var buttonSource = buttonSources[i];

                var button = new ContextButton(owner,buttonSource) { Sizing = Sizing.Fill };
                buttonDictionary[buttonSource.InputMode] = button;

                parent.AddChild(button);
                buttonGroup.AddChild(parent);
            }

            owner.UIState.AddChild(panel);
            panel.AddChild(buttonGroup);
        }

    }
}
