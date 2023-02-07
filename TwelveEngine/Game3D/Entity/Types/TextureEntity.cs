namespace TwelveEngine.Game3D.Entity.Types {
    public class TextureEntity:TextureRectangle {

        private void BindEvents() => OnLoad += LoadTexture;

        private Texture2D pendingTexture = null;

        public TextureEntity(string textureName) {
            TextureName = textureName;
            BindEvents();
        }

        public TextureEntity(Texture2D texture) {
            pendingTexture = texture;
            BindEvents();
        }

        public void SetUVArea(int x,int y,int width,int height) {
            SetUVArea(new Rectangle(x,y,width,height));
        }

        public void SetUVArea(int x,int y,Point size) {
            SetUVArea(new Rectangle(x,y,size.X,size.Y));
        }

        public void SetUVArea(int x,int y,Vector2 size) {
            SetUVArea(new Rectangle(x,y,(int)size.X,(int)size.Y));
        }

        public void SetUVArea(Rectangle textureArea) {
            var texture = Texture ?? pendingTexture;
            if(texture == null) {
                UVTopLeft = Vector2.Zero;
                UVBottomRight = Vector2.Zero;
                return;
            }
            float textureWidth = texture.Width, textureHeight = texture.Height;
            UVTopLeft = new Vector2(textureArea.X / textureWidth,textureArea.Y / textureHeight);
            UVBottomRight = new Vector2(textureArea.Right / textureWidth,textureArea.Bottom / textureHeight);
        }

        public TextureEntity() => BindEvents();

        private string _textureName = null;
        public string TextureName {
            get => _textureName;
            set {
                if(_textureName == value) {
                    return;
                }
                _textureName = value;
            }
        }

        private void LoadTexture() {
            if(_textureName != null) {
                Texture = Content.Load<Texture2D>(TextureName);
            } else if(pendingTexture != null) {
                Texture = pendingTexture;
                pendingTexture = null;
            }
        }
    }
}
