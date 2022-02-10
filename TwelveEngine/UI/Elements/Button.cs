using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine.UI.Elements {
    public class Button:Panel {
        public Button(Color color) : base(color) {
            OnMouseDown += point => OnClick(this);
            Padding = 4;
            IsInteractable = true;
        }
        public event Action<Button> OnClick;
    }
}
