using System;
using System.Collections.Generic;

namespace TwelveEngine.UI {
    public class Element {

        protected Positioning positioning = Positioning.Absolute;
        protected Sizing sizing = Sizing.Absolute;

        protected int width = 0, height = 0;
        protected int x = 0, y = 0;

        private Element parent = null;
        private bool hasParent() => parent != null;

        private readonly List<Element> children = new List<Element>();

        internal event Action LayoutChanged;
        private bool layoutChangesEnabled = false;

        public void AddChild(Element child) {
            children.Add(child);
            child.parent = this;
            child.FireLayoutChanged();
        }

        public void RemoveChild(Element child) {
            children.Remove(child);
            child.parent = null;
            child.FireLayoutChanged();
        }

        private void enableLayoutChanges() {
            layoutChangesEnabled = true;
            foreach(Element child in children) {
                child.enableLayoutChanges();
            }
        }
        private void pauseLayoutChanges() {
            layoutChangesEnabled = false;
            foreach(Element child in children) {
                child.pauseLayoutChanges();
            }
        }

        public void StartLayout() {
            enableLayoutChanges();
            FireLayoutChanged();
        }
        public void PauseLayout() {
            pauseLayoutChanges();
        }

        protected void FireLayoutChanged() {
            if(!layoutChangesEnabled) {
                return;
            }
            LayoutChanged?.Invoke();
            foreach(Element child in children) {
                child.FireLayoutChanged();
            }
        }

        protected (int X,int Y) GetAbsolutePosition() {
            return (x + leftPadding, y - rightPadding);
        }

        protected virtual (int X,int Y) GetCenteredPosition() {
            /* TODO: Be corrected to pixel perfection */
            int x = parent.x + parent.Width / 2 - this.x + width / 2;
            int y = parent.y + parent.Height / 2 - this.y + height / 2;
            return (x + leftPadding, y + topPadding);
        }

        protected virtual (int X,int Y) GetRelativePosition() {
            return (parent.x + x + leftPadding, parent.y + y + topPadding);
        }

        protected virtual (int Width,int Height) GetAbsoluteSize() {
            return (width - rightPadding, height - bottomPadding);
        }

        protected virtual (int Width,int Height) GetFillSize() {
            return (parent.Width - rightPadding, parent.Height - bottomPadding);
        }

        protected virtual (int Width,int Height) GetBoxFill() {
            int size = parent.Width > parent.Height ? parent.Height : parent.Width;
            return (size - rightPadding, size - bottomPadding);
        }

        public (int Width,int Height) GetSize() {
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

        public (int X, int Y) GetPosition() {
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
                FireLayoutChanged();
            }
        }
        public int LeftPadding {
            get => leftPadding;
            set {
                if(leftPadding == value) {
                    return;
                }
                leftPadding = value;
                FireLayoutChanged();
            }
        }
        public int RightPadding {
            get => rightPadding;
            set {
                if(rightPadding == value) {
                    return;
                }
                rightPadding = value;
                FireLayoutChanged();
            }
        }
        public int TopPadding {
            get => topPadding;
            set {
                if(topPadding == value) {
                    return;
                }
                topPadding = value;
                FireLayoutChanged();
            }
        }
        public int BottomPadding {
            get => bottomPadding;
            set {
                if(bottomPadding == value) {
                    return;
                }
                bottomPadding = value;
                FireLayoutChanged();
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
            get => GetSize().Width;
            set {
                if(width == value) {
                    return;
                }
                width = value;
                FireLayoutChanged();
            }
        }
        public int Height {
            get => GetSize().Height;
            set {
                if(height == value) {
                    return;
                }
                height = value;
                FireLayoutChanged();
            }
        }
        public int X {
            get => GetPosition().X;
            set {
                if(x == value) {
                    return;
                }
                x = value;
                FireLayoutChanged();
            }
        }
        public int Y {
            get => GetPosition().Y;
            set {
                if(y == value) {
                    return;
                }
                y = value;
                FireLayoutChanged();
            }
        }
        public Sizing Sizing {
            get => sizing;
            set {
                if(sizing == value) {
                    return;
                }
                sizing = value;
                FireLayoutChanged();
            }
        }
        public Positioning Positioning {
            get => positioning;
            set {
                if(positioning == value) {
                    return;
                }
                positioning = value;
                FireLayoutChanged();
            }
        }
    }
}
