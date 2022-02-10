using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.UI.Elements {
    public class ImageButton:ImagePanel {
        public ImageButton(string imageName,Rectangle? source = null):base(imageName,source) {
            IsInteractable = true;
            OnMouseDown += _ => OnClick?.Invoke(this);
        }
        public ImageButton(Texture2D texture,Rectangle? source = null) : base(texture,source) {
            IsInteractable = true;
            OnMouseDown += _ => OnClick?.Invoke(this);
        }
        public event Action<ImageButton> OnClick;
    }
}
