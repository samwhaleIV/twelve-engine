using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine;
using TwelveEngine.Shell;
using TwelveEngine.UI;
using TwelveEngine.UI.Book;

namespace John {

    using static CollectionGameUI;
    using static Constants;

    public sealed class CollectionGameUI:SpriteBook {

        public Texture2D Texture => Game.TileMapTexture;
        public CollectionGame Game { get; private init; }

        public SpriteElement GameTitle { get; private init; }
        public SpriteElement JohnWearsLabel { get; private init; }
        public SpriteElement JohnClothingGuide {  get; private init; }

        public ScoreBufferCell[] ScoreBufferContainer { get; private init; }

        public VirtualDPad VirtualDPad { get; private init; }

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

            VirtualDPad = new VirtualDPad() {
                Game = game,
                Texture = Texture,
                PositionMode = CoordinateMode.Absolute,
                SizeMode = CoordinateMode.Absolute,
                TextureSource = new Rectangle(0,160,48,48)
            };

            AddElement(VirtualDPad);

            ScoreBufferContainer = new ScoreBufferCell[ROUND_COMPLETION_COUNT];
            for(int i = 0;i<ScoreBufferContainer.Length;i++) {
                ScoreBufferContainer[i] = GetScoreBufferCell();
            }

            SetFirstPage(new CollectionGamePage(this));
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
            Rectangle source = new Rectangle(JohnDecorator.GetTextureOrigin(JohnConfig.Value),new Point(JOHN_WIDTH,JOHN_HEIGHT));

            destination.Position += new Vector2(PixelSize);
            destination.Size -= new Vector2(PixelSize * 2);

            spriteBatch.Draw(JohnDecorator.Texture,(Rectangle)destination,source,ComputedColor,rotation,origin,SpriteEffects.None,Depth + 0.1f);
        }

        public int? JohnConfig { get; set; }
        public float PixelSize { get; set; }
    }

    public sealed class VirtualDPad:SpriteElement {
        public CollectionGame Game { get; set; }

        protected override void Draw(SpriteBatch spriteBatch,Texture2D texture,Rectangle sourceArea) {
            FloatRectangle destination = ComputedArea;

            destination.Size = Vector2.Round(Size);
            destination.Position += destination.Size * 0.5f;

            Vector2 origin = sourceArea.Size.ToVector2() * 0.5f;

            float rotation = MathHelper.ToRadians(ComputedRotation);

            spriteBatch.Draw(texture,(Rectangle)destination,sourceArea,ComputedColor,rotation,origin,SpriteEffects.None,Depth);

            sourceArea.Size = new Point(16,16);
            sourceArea.Location += new Point(64,0);
            destination.Size *= (float)1 / 3;

            if(Game.VirtualDPad.Up) {
                spriteBatch.Draw(texture,(Rectangle)new FloatRectangle(destination.Position+new Vector2(destination.Size.X,0),destination.Size),new Rectangle(64,160,16,16),ComputedColor,rotation,origin,SpriteEffects.None,Depth);
            }
            if(Game.VirtualDPad.Left) {
                spriteBatch.Draw(texture,(Rectangle)new FloatRectangle(destination.Position+new Vector2(0,destination.Size.Y),destination.Size),new Rectangle(48,176,16,16),ComputedColor,rotation,origin,SpriteEffects.None,Depth);
            }
            if(Game.VirtualDPad.Down) {
                spriteBatch.Draw(texture,(Rectangle)new FloatRectangle(destination.Position+new Vector2(destination.Size.X,destination.Size.Y*2),destination.Size),new Rectangle(64,192,16,16),ComputedColor,rotation,origin,SpriteEffects.None,Depth);
            }
            if(Game.VirtualDPad.Right) {
                spriteBatch.Draw(texture,(Rectangle)new FloatRectangle(destination.Position+new Vector2(destination.Size.X*2,destination.Size.Y),destination.Size),new Rectangle(80,176,16,16),ComputedColor,rotation,origin,SpriteEffects.None,Depth);
            }
            if(Game.VirtualDPad.Action) {
                spriteBatch.Draw(texture,(Rectangle)new FloatRectangle(destination.Position+new Vector2(destination.Size.X,destination.Size.Y),destination.Size),new Rectangle(64,176,16,16),ComputedColor,rotation,origin,SpriteEffects.None,Depth);
            }
        }
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

            var virtualDPad = _book.VirtualDPad;
            virtualDPad.Scale = 1;
            virtualDPad.Flags = ElementFlags.Update;

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

            float pixelScale = Game.Camera.Scale;

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

            var virtualDPad = _book.VirtualDPad;
            virtualDPad.Size = virtualDPad.SourceSize * pixelScale;
            virtualDPad.Position = Viewport.Size - new Vector2(screenMargin) - virtualDPad.Size;
        }
    }
}
