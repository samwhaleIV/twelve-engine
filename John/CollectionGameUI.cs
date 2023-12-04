using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine.Shell;
using TwelveEngine.UI.Book;

namespace John {

    using static CollectionGameUI;

    public sealed class CollectionGameUI:SpriteBook {

        public Texture2D Texture => Game.TileMapTexture;
        public JohnCollectionGame Game { get; private init; }

        public SpriteElement GameTitle { get; private init; }
        public SpriteElement JohnWearsLabel { get; private init; }
        public SpriteElement JohnClothingGuide {  get; private init; }


        public static readonly Rectangle ShirtSource = new Rectangle(32,240,16,16), PantsSource = new Rectangle(49,240,14,16), HairSource = new Rectangle(64,240,16,16);
        public static readonly Rectangle FindJohnTextSource = new Rectangle(48,208,64,12), KillFakesTextSource = new Rectangle(112,208,58,12);

        public CollectionGameUI(JohnCollectionGame game) : base(game) {
            Game = game;
            GameTitle = AddElement(new SpriteElement() {
                Texture = Texture,

                PositionModeX = CoordinateMode.Relative,
                PositionModeY = CoordinateMode.Absolute,

                SizeMode = CoordinateMode.Absolute,
                Offset = new Vector2(-0.5f,0f)
            });
            JohnWearsLabel = AddElement(new SpriteElement() {
                Texture = Texture,
                TextureSource = new Rectangle(48,224,45,7),

                PositionModeX = CoordinateMode.Absolute,
                PositionModeY = CoordinateMode.Absolute,

                SizeMode = CoordinateMode.Absolute,
                Offset = new Vector2(0,-0.5f)
            });
            JohnClothingGuide = AddElement(new SpriteElement() {
                Texture = Texture,

                PositionModeX = CoordinateMode.Absolute,
                PositionModeY = CoordinateMode.Absolute,

                SizeMode = CoordinateMode.Absolute,
                Offset = new Vector2(0,-0.5f)          
            });
            SetFirstPage(new CollectionGamePage(this));
        }
    }

    public sealed class CollectionGamePage:BookPage<SpriteElement> {

        private readonly CollectionGameUI _book;

        private JohnCollectionGame Game => _book.Game;

        public CollectionGamePage(CollectionGameUI book) {
            _book = book;
        }

        public override void Close() {
            throw new NotImplementedException();
        }

        public override BookElement Open() {
            var title = _book.GameTitle;
            var johnWearsLabel = _book.JohnWearsLabel;
            var clothingGuide = _book.JohnClothingGuide;

            title.Scale = 1f;
            title.Flags = ElementFlags.Update;

            johnWearsLabel.Scale = 1;
            johnWearsLabel.Flags = ElementFlags.Update;

            clothingGuide.Scale = 1;
            clothingGuide.Flags = ElementFlags.Update;

            return null;
        }

        private void UpdateClothingGuide() {
            var clothingGuide = _book.JohnClothingGuide;
            clothingGuide.Color = Game.RealJohnMask.Color;
            clothingGuide.TextureSource = Game.RealJohnMask.Type switch {
                JohnMatchType.Hair => HairSource,
                JohnMatchType.Shirt => ShirtSource,
                JohnMatchType.Pants => PantsSource,
                _ => throw new NotImplementedException()
            };
        }

        private void UpdateGameTitle() {
            _book.GameTitle.TextureSource = Game.FindRealJohnMode ? FindJohnTextSource : KillFakesTextSource;
        }

        public override void Update() {
            UpdateGameTitle();
            UpdateClothingGuide();
            var title = _book.GameTitle;
            var johnWearsLabel = _book.JohnWearsLabel;
            var clothingGuide = _book.JohnClothingGuide;

            float pixelScale = Game.GetUIScale();

            float screenMargin = pixelScale * 1f;

            title.Position = new Vector2(0.5f,screenMargin);
            title.Size = title.SourceSize * pixelScale;

            johnWearsLabel.Size = johnWearsLabel.SourceSize * pixelScale;
            clothingGuide.Size = clothingGuide.SourceSize * pixelScale;
        
            float clothingGuideRowWidth = johnWearsLabel.Size.X + clothingGuide.Size.X + screenMargin;

            float xCenter = Viewport.Width * 0.5f + clothingGuideRowWidth * 0.25f;
            float clothingGuideRowCenter = title.Position.Y + title.Size.Y + screenMargin + clothingGuide.Size.Y * 0.5f;

            float halfMargin = screenMargin * 0.5f;
            johnWearsLabel.Position = new Vector2(xCenter-johnWearsLabel.Size.X-halfMargin,clothingGuideRowCenter);
            clothingGuide.Position = new Vector2(xCenter+halfMargin,clothingGuideRowCenter);
        }
    }
}
