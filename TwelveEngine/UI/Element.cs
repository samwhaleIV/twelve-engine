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
            return (x + leftPadding, y + topPadding);
        }

        protected virtual (int X,int Y) GetCenteredPosition() {
            float x = parent.screenX + (parent.screenWidth / 2f) - (width / 2f);
            float y = parent.screenY + (parent.screenHeight / 2f) - (height / 2f);
            return ((int)Math.Floor(x), (int)Math.Floor(y));
        }

        protected virtual (int X,int Y) GetRelativePosition() {
            return (parent.screenX + x + leftPadding, parent.screenY + y + topPadding);
        }

        protected virtual (int Width,int Height) GetAbsoluteSize() {
            return (width - rightPadding - leftPadding, height - bottomPadding - topPadding);
        }

        protected virtual (int Width,int Height) GetFillSize() {
            return (parent.screenWidth - rightPadding - leftPadding, parent.screenHeight - bottomPadding - topPadding);
        }

        protected virtual (int Width,int Height) GetBoxFill() {
            int size = parent.screenWidth > parent.screenHeight ? parent.screenHeight : parent.screenWidth;
            return (size - rightPadding - leftPadding, size - bottomPadding - topPadding);
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

        protected int leftPadding = 0, rightPadding = 0,
                      topPadding = 0, bottomPadding = 0;

        public int Padding {
            set {
                leftPadding = value;
                rightPadding = value;
                topPadding = value;
                bottomPadding = value;
                UpdateLayout();
            }
        }
        public int PaddingLeft {
            get => leftPadding;
            set {
                if(leftPadding == value) {
                    return;
                }
                leftPadding = value;
                UpdateLayout();
            }
        }
        public int PaddingRight {
            get => rightPadding;
            set {
                if(rightPadding == value) {
                    return;
                }
                rightPadding = value;
                UpdateLayout();
            }
        }
        public int PaddingTop {
            get => topPadding;
            set {
                if(topPadding == value) {
                    return;
                }
                topPadding = value;
                UpdateLayout();
            }
        }
        public int PaddingBottom {
            get => bottomPadding;
            set {
                if(bottomPadding == value) {
                    return;
                }
                bottomPadding = value;
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
    }
}
