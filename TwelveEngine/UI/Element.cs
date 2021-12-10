using System;
using System.Collections.Generic;

namespace TwelveEngine.UI {
    public class Element {

        protected Positioning positioning = Positioning.Absolute;
        protected Sizing sizing = Sizing.Absolute;

        protected int width = 0, height = 0, x = 0, y = 0;
        protected int screenWidth = 0, screenHeight = 0, screenX = 0, screenY = 0;

        private Element parent = null;
        private bool hasParent() => parent != null;

        private readonly List<Element> children = new List<Element>();
        public Element[] GetChildren() => children.ToArray();

        protected event Action LayoutUpdated;
        private bool layoutUpdatesEnabled = false;

        internal virtual void AddChild(Element child,bool updateLayout = true) {
            children.Add(child);
            child.parent = this;
            if(!updateLayout) {
                return;
            }
            child.UpdateLayout();
        }

        public virtual void AddChild(Element child) {
            AddChild(child,updateLayout: true);
        }

        public void RemoveChild(Element child) {
            children.Remove(child);
            child.parent = null;
            child.UpdateLayout();
        }

        private void enableLayoutChanges() {
            layoutUpdatesEnabled = true;
            foreach(Element child in children) {
                child.enableLayoutChanges();
            }
        }
        private void pauseLayoutChanges() {
            layoutUpdatesEnabled = false;
            foreach(Element child in children) {
                child.pauseLayoutChanges();
            }
        }

        public bool LayoutUpdatesEnabled => layoutUpdatesEnabled;

        public void StartLayout() {
            enableLayoutChanges();
            UpdateLayout();
        }
        public void PauseLayout() {
            pauseLayoutChanges();
        }

        protected void UpdateLayout() {
            if(!layoutUpdatesEnabled) {
                return;
            }
            cacheSize(getSize());
            cachePosition(getPosition());
            LayoutUpdated?.Invoke();
            foreach(Element child in children) {
                child.UpdateLayout();
            }
        }

        protected (int X,int Y) GetAbsolutePosition() {
            return (x + paddingLeft, y + paddingTop);
        }

        protected virtual (int X,int Y) GetCenteredPosition() {
            float x = parent.screenX + (parent.screenWidth / 2f) - (screenWidth / 2f);
            float y = parent.screenY + (parent.screenHeight / 2f) - (screenHeight / 2f);
            return ((int)Math.Floor(x) + this.x + paddingLeft, (int)Math.Floor(y) + this.y + paddingRight);
        }

        protected virtual (int X,int Y) GetRelativePosition() {
            return (parent.screenX + x + paddingLeft, parent.screenY + y + paddingTop);
        }

        protected virtual (int Width,int Height) GetAbsoluteSize() {
            return (width - paddingRight - paddingLeft, height - paddingBottom - paddingTop);
        }

        protected virtual (int Width,int Height) GetFillSize() {
            return (parent.screenWidth - paddingRight - paddingLeft, parent.screenHeight - paddingBottom - paddingTop);
        }

        protected virtual (int Width,int Height) GetBoxFill() {
            int size = parent.screenWidth > parent.screenHeight ? parent.screenHeight : parent.screenWidth;
            return (size - paddingRight - paddingLeft, size - paddingBottom - paddingTop);
        }

        private (int Width, int Height) cacheSize((int Width, int Height) size) {
            screenWidth = size.Width;
            screenHeight = size.Height;
            return size;
        }
        private (int X,int Y) cachePosition((int X,int Y) position) {
            screenX = position.X;
            screenY = position.Y;
            return position;
        }

        private (int Width,int Height) getSize() {
            switch(sizing) {
                case Sizing.Absolute: default:
                    return GetAbsoluteSize();
                case Sizing.Fill:
                    if(!hasParent()) goto default;
                    return GetFillSize();
                case Sizing.BoxFill:
                    if(!hasParent()) goto default;
                    return GetBoxFill();
            }
        }

        private (int X, int Y) getPosition() {
            switch(positioning) {
                case Positioning.Absolute:
                default: {
                    return GetAbsolutePosition();
                }
                case Positioning.Relative: {
                    if(!hasParent()) goto default;
                    return GetRelativePosition();
                }
                case Positioning.Centered: {
                    if(!hasParent()) goto default;
                    return GetCenteredPosition();
                }
            }
        }

        protected int paddingLeft = 0, paddingRight = 0,
                      paddingTop = 0, paddingBottom = 0;

        public int Padding {
            set {
                paddingLeft = value;
                paddingRight = value;
                paddingTop = value;
                paddingBottom = value;
                UpdateLayout();
            }
        }
        public int PaddingLeft {
            get => paddingLeft;
            set {
                if(paddingLeft == value) {
                    return;
                }
                paddingLeft = value;
                UpdateLayout();
            }
        }
        public int PaddingRight {
            get => paddingRight;
            set {
                if(paddingRight == value) {
                    return;
                }
                paddingRight = value;
                UpdateLayout();
            }
        }
        public int PaddingTop {
            get => paddingTop;
            set {
                if(paddingTop == value) {
                    return;
                }
                paddingTop = value;
                UpdateLayout();
            }
        }
        public int PaddingBottom {
            get => paddingBottom;
            set {
                if(paddingBottom == value) {
                    return;
                }
                paddingBottom = value;
                UpdateLayout();
            }
        }
        public Element Parent {
            get => parent;
            set {
                if(parent == value) {
                    return;
                }
                parent.RemoveChild(value);
                value.AddChild(this);
            }
        }
        public int Width {
            get => width;
            set {
                if(width == value) {
                    return;
                }
                width = value;
                UpdateLayout();
            }
        }
        public int Height {
            get => height;
            set {
                if(height == value) {
                    return;
                }
                height = value;
                UpdateLayout();
            }
        }
        public int X {
            get => x;
            set {
                if(x == value) {
                    return;
                }
                x = value;
                UpdateLayout();
            }
        }
        public int Y {
            get => y;
            set {
                if(y == value) {
                    return;
                }
                y = value;
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

        public int ScreenWidth => screenWidth;
        public int ScreenHeight => screenHeight;
        public int ScreenX => screenX;
        public int ScreenY => screenY;
    }
}
