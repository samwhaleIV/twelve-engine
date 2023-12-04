using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine;
using TwelveEngine.Shell;
using TwelveEngine.UI.Book;

namespace John {

    using static CollectionGameUI;

    public sealed class CollectionGameUI:SpriteBook {

        public Texture2D Texture => Game.TileMapTexture;
        public CollectionGame Game { get; private init; }

        public SpriteElement GameTitle { get; private init; }
        public SpriteElement JohnWearsLabel { get; private init; }
        public SpriteElement JohnClothingGuide {  get; private init; }

        public ScoreBufferCell[] ScoreBufferContainer { get; private init; }

        public static readonly Rectangle ShirtSource = new Rectangle(32,240,16,16), PantsSource = new Rectangle(49,240,14,16), HairSource = new Rectangle(64,240,16,16);
        public static readonly Rectangle FindJohnTextSource = new Rectangle(48,208,64,12), KillFakesTextSource = new Rectangle(112,208,58,12);

        private ScoreBufferCell GetScoreBufferCell() {
            var cell = new ScoreBufferCell() {
                Texture = Texture,
                TextureSource = new Rectangle(0,208,11,18),
                SizeMode = CoordinateMode.Absolute,
                PositionMode = CoordinateMode.Absolute,
                JohnDecorator = Game.Decorator
            };
            AddElement(cell);
            return cell;
        }

        public CollectionGameUI(CollectionGame game) : base(game) {
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

            ScoreBufferContainer = new ScoreBufferCell[Constants.ROUND_COMPLETION_COUNT];
            for(int i = 0;i<ScoreBufferContainer.Length;i++) {
                ScoreBufferContainer[i] = GetScoreBufferCell();
            }

            SetFirstPage(new CollectionGamePage(this));

            game.JohnSaved += Game_JohnSaved;
            game.IncorrectJohnSaved += Game_IncorrectJohnSaved;
            game.RealJohnKilled += Game_RealJohnKilled;
            game.ImposterKilled += Game_ImposterKilled;
            game.WrongBin += Game_WrongBin; 
        }

        private void Game_WrongBin() {

        }

        private void Game_ImposterKilled() {
            
        }

        private void Game_RealJohnKilled() {
            
        }

        private void Game_IncorrectJohnSaved() {
            
        }

        private void Game_JohnSaved() {
            
        }
    }

    public sealed class ScoreBufferCell:SpriteElement {

        public JohnDecorator JohnDecorator { get; init; }

        protected override void Draw(SpriteBatch spriteBatch,Texture2D texture,Rectangle sourceArea) {
            FloatRectangle destination = ComputedArea;

            destination.Size = Vector2.Round(Size);
            destination.Position += destination.Size * 0.5f;

            Vector2 origin = sourceArea.Size.ToVector2() * 0.5f;

            float rotation = MathHelper.ToRadians(ComputedRotation);

            spriteBatch.Draw(texture,(Rectangle)destination,sourceArea,ComputedColor,rotation,origin,SpriteEffects.None,Depth);

            if(!JohnConfig.HasValue) {
                return;
            }
            Rectangle source = new Rectangle(JohnDecorator.GetTextureOrigin(JohnConfig.Value),new Point(9,16));

            destination.Position = destination.Position + new Vector2(PixelSize);
            destination.Size = destination.Size - new Vector2(PixelSize * 2);

            spriteBatch.Draw(JohnDecorator.Texture,(Rectangle)destination,source,ComputedColor,rotation,origin,SpriteEffects.None,Depth + 0.1f);
        }

        public int? JohnConfig { get; set; }
        public float PixelSize { get; set; }
    }

    public sealed class CollectionGamePage:BookPage<SpriteElement> {

        private readonly CollectionGameUI _book;

        private CollectionGame Game => _book.Game;

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

            foreach(var cell in _book.ScoreBufferContainer) {
                cell.Scale = 1;
                cell.Flags = ElementFlags.Update;
            }

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

            Vector2 scoreCellSize = _book.ScoreBufferContainer[0].TextureSource.Size.ToVector2() * pixelScale;

            float totalCellWidth = _book.ScoreBufferContainer.Length * scoreCellSize.X + (_book.ScoreBufferContainer.Length-1) * screenMargin;

            float cellY = Viewport.Height - scoreCellSize.Y - screenMargin;
            float cellX = Viewport.Width * 0.5f - totalCellWidth * 0.5f;

            for(int i = 0;i<_book.ScoreBufferContainer.Length;i++) {
                var cell = _book.ScoreBufferContainer[i];
                cell.Size = scoreCellSize;
                cell.Position = new Vector2(cellX,cellY);
                cellX += screenMargin + scoreCellSize.X;
                cell.PixelSize = pixelScale;
                cell.JohnConfig = i < Game.BinBuffer.Count ? Game.BinBuffer[i] : null;
            }
        }
    }
}
