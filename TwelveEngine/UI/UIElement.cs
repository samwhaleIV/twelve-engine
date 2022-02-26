using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TwelveEngine.UI {
    public class UIElement {

        protected Positioning positioning = Positioning.Normal;
        protected Sizing sizing = Sizing.Normal;

        private Rectangle area = Rectangle.Empty;
        private Rectangle computedArea = Rectangle.Empty;

        private Anchor anchor = Anchor.TopLeft;
        private Padding padding = new Padding();

        private UIElement parent = null;
        public bool HasParent() => parent != null;

        private readonly List<UIElement> children = new List<UIElement>();
        public List<UIElement> Children => children;

        private int layoutFreezes = 1;
        public bool LayoutUpdatesEnabled => layoutFreezes < 1;

        protected event Action LayoutUpdated;

        public void AddChild(params UIElement[] children) {
            foreach(var child in children) {
                this.children.Add(child);
                child.parent = this;
                child.UpdateLayout();
            }
        }

        public void RemoveChild(UIElement child) {
            children.Remove(child);
            child.parent = null;
            child.UpdateLayout();
        }

        internal void ForceLayout() {
            layoutFreezes = 1;
            PopLayoutFreeze();
        }

        public void PopLayoutFreeze() {
            if(layoutFreezes == 0) {
                return;
            }
            layoutFreezes -= 1;
            if(layoutFreezes != 0) {
                return;
            }
            UpdateLayout();
        }
        public void PushLayoutFreeze() {
            layoutFreezes++;
        }

        private void cacheSize(Point size) {
            computedArea.Width = size.X;
            computedArea.Height = size.Y;
        }
        private void cachePosition(Point position) {
            computedArea.X = position.X;
            computedArea.Y = position.Y;
        }

        protected void UpdateLayout() {
            if(!LayoutUpdatesEnabled) return;

            var size = getSize();
            size.X -= PaddingLeft + PaddingRight;
            size.Y -= PaddingTop + PaddingBottom;

            var position = getPosition();
            position.X += PaddingLeft;
            position.Y += PaddingTop;

            cacheSize(size);
            cachePosition(position);

            LayoutUpdated?.Invoke();
            foreach(UIElement child in children) {
                child.UpdateLayout();
            }
        }

        protected virtual Point GetPosition() => area.Location;

        protected virtual Point GetSize() => area.Size;

        protected virtual Point GetFillSize() => parent.computedArea.Size;

        protected virtual Point GetBoxFill() {
            return new Point(Math.Max(parent.ComputedWidth,parent.ComputedHeight));
        }

        private int getFractionalWidth() {
            return (int)(parent.ComputedWidth * (Width / 100f));
        }

        private int getFractionalHeight() {
            return (int)(parent.ComputedHeight * (Height / 100f));
        }

        private int getCenteredX() {
            return (int)(parent.ComputedX + parent.ComputedWidth * 0.5f - ComputedWidth * 0.5f);
        }
        
        private int getCenteredY() {
            return (int)(parent.ComputedY + parent.ComputedHeight * 0.5f - ComputedHeight * 0.5f);
        }

        private int horizontalAnchor(int x) {
            if(anchor == Anchor.TopLeft || anchor == Anchor.BottomLeft) {
                return parent.ComputedX + x;
            } else {
                return parent.ComputedArea.Right - ComputedWidth - x;
            }
        }

        private int verticalAnchor(int y) {
            if(anchor == Anchor.TopRight || anchor == Anchor.TopLeft) {
                return parent.ComputedY + y;
            } else {
                return parent.ComputedArea.Bottom - ComputedHeight - y;
            }
        }

        private int getCenteredXOrigin(int x) {
            return (int)(x - ComputedWidth * 0.5f);
        }

        private int getCenteredYOrigin(int y) {
            return (int)(y - ComputedHeight * 0.5f);
        }

        private Point getRelativeSize() {
            var size = GetSize();
            switch(sizing) {
                case Sizing.Fill:
                    size = GetFillSize();
                    break;
                case Sizing.BoxFill:
                    size = GetBoxFill();
                    break;
                case Sizing.Percent:
                    size.X = getFractionalWidth();
                    size.Y = getFractionalHeight();
                    break;
                case Sizing.PercentX:
                    size.X = getFractionalWidth();
                    break;
                case Sizing.PercentY:
                    size.Y = getFractionalHeight();
                    break;
            }
            return size;
        }

        private Point getSize() {
            if(HasParent()) {
                return getRelativeSize();
            }
            return GetSize();
        }

        private Point getRelativePosition() {
            var position = GetPosition();
            switch(positioning) {
                case Positioning.Fixed:
                    position.X = X;
                    position.Y = Y;
                    break;
                case Positioning.Normal:
                    position.X = horizontalAnchor(position.X);
                    position.Y = verticalAnchor(position.Y);
                    break;
                case Positioning.CenterParent:
                    position.X = getCenteredX();
                    position.Y = getCenteredY();
                    break;
                case Positioning.CenterParentX: 
                    position.X = getCenteredX();
                    position.Y = verticalAnchor(position.Y);
                    break;
                case Positioning.CenterParentY:
                    position.X = horizontalAnchor(position.X);
                    position.Y = getCenteredY();
                    break;
                /* Is the compiler smart enough to expand this into multiple cases? Fuck it, who knows! */
                case Positioning.Center:
                case Positioning.CenterX:
                case Positioning.CenterY:
                    position.X = horizontalAnchor(position.X);
                    position.Y = verticalAnchor(position.Y);
                    if(positioning == Positioning.Center || positioning == Positioning.CenterX) {
                        position.X = getCenteredXOrigin(position.X);
                    }
                    if(positioning == Positioning.Center || positioning == Positioning.CenterY) {
                        position.Y = getCenteredYOrigin(position.Y);
                    }
                    break;
            }
            return position;
        }

        private Point getPosition() => HasParent() ? getRelativePosition() : GetPosition();

        public void SetSize(Point size) {
            if(size == area.Size) {
                return;
            }
            area.Size = size;
            UpdateLayout();
        }

        internal UIElement Parent {
            get => parent;
            set {
                if(parent == value) {
                    return;
                }
                parent.RemoveChild(this);
                value.AddChild(this);
            }
        }

        public int ComputedX {
            get => computedArea.X;
        }
        public int ComputedY {
            get => computedArea.Y;
        }
        public int ComputedWidth {
            get => computedArea.Width;
        }
        public int ComputedHeight {
            get => computedArea.Height;
        }
        public Rectangle ComputedArea {
            get => computedArea;
        }
        public Rectangle Area {
            get => area;
            set {
                if(area == value) {
                    return;
                }
                area = value;
                UpdateLayout();
            }
        }
        public Anchor Anchor {
            get => anchor;
            set {
                if(anchor == value) {
                    return;
                }
                anchor = value;
                UpdateLayout();
            }
        }
        public int Padding {
            set {
                if(padding.IsBalanced && padding.Value == value) {
                    return;
                }
                padding = new Padding(value);
                UpdateLayout();
            }
        }
        public void SetPadding(Padding padding) {
            if(this.padding == padding) {
                return;
            }
            this.padding = padding;
            UpdateLayout();
        }
        public Padding GetPadding() {
            return padding;
        }
        public int PaddingLeft {
            get => padding.Left;
            set {
                if(padding.Left == value) {
                    return;
                }
                padding.Left = value;
                UpdateLayout();
            }
        }
        public int PaddingRight {
            get => padding.Right;
            set {
                if(padding.Right == value) {
                    return;
                }
                padding.Right = value;
                UpdateLayout();
            }
        }
        public int PaddingTop {
            get => padding.Top;
            set {
                if(padding.Top == value) {
                    return;
                }
                padding.Top = value;
                UpdateLayout();
            }
        }
        public int PaddingBottom {
            get => padding.Bottom;
            set {
                if(padding.Bottom == value) {
                    return;
                }
                padding.Bottom = value;
                UpdateLayout();
            }
        }
        public int Width {
            get => area.Width;
            set {
                if(area.Width == value) {
                    return;
                }
                area.Width = value;
                UpdateLayout();
            }
        }
        public int Height {
            get => area.Height;
            set {
                if(area.Height == value) {
                    return;
                }
                area.Height = value;
                UpdateLayout();
            }
        }
        public int X {
            get => area.X;
            set {
                if(area.X == value) {
                    return;
                }
                area.X = value;
                UpdateLayout();
            }
        }
        public int Y {
            get => area.Y;
            set {
                if(area.Y == value) {
                    return;
                }
                area.Y = value;
                UpdateLayout();
            }
        }
        public Sizing Sizing {
            get => sizing;
            set {
                if(sizing == value) {
                    return;
                }
                sizing = value;
                UpdateLayout();
            }
        }
        public Positioning Positioning {
            get => positioning;
            set {
                if(positioning == value) {
                    return;
                }
                positioning = value;
                UpdateLayout();
            }
        }
        public void SwapOrientation() {
            PushLayoutFreeze();
            int buffer = Width;
            Width = Height;
            Height = buffer;
            PopLayoutFreeze();
        }
    }
}
