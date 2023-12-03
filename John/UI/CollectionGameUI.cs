using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Shell;
using TwelveEngine.UI.Book;

namespace John.UI {
    public sealed class CollectionGameUI:SpriteBook {

        public Texture2D Texture { get; private init; }
        public SpriteElement TestElement { get; private init; }

        public CollectionGameUI(Texture2D texture,InputGameState gameState) : base(gameState) {
            Texture = texture;
            TestElement = AddElement(new SpriteElement() {
                Texture = Texture,
                TextureSource = new Rectangle(0,16,16,16),
                PositionMode = CoordinateMode.Relative,
                SizeMode = CoordinateMode.Absolute,
                Offset = new Vector2(-0.5f)
            });
            SetFirstPage(new CollectionGamePage(this));
        }
    }

    public sealed class CollectionGamePage:BookPage<SpriteElement> {

        private readonly CollectionGameUI _book;

        public CollectionGamePage(CollectionGameUI book) {
            _book = book;
        }

        public override void Close() {

        }

        public override BookElement Open() {
            _book.TestElement.Scale = 1;
            _book.TestElement.Flags = ElementFlags.Update;
            return _book.TestElement;
        }

        public override void Update() {
            _book.TestElement.Position = new Vector2(0.5f,0.5f);
            _book.TestElement.Size = new Vector2(100,100);
        }
    }
}
