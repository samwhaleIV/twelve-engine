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
            return (x, y);
        }

        protected virtual (int X,int Y) GetCenteredPosition() {
            /* TODO: Be corrected to pixel perfection */
            int x = parent.x + parent.Width / 2 - this.x + width / 2;
            int y = parent.y + parent.Height / 2 - this.y + height / 2;
            return (x, y);
        }

        protected virtual (int X,int Y) GetRelativePosition() {
            return (parent.x + x, parent.y + y);
        }

        protected virtual (int Width,int Height) GetAbsoluteSize() {
            return (width, height);
        }

        protected virtual (int Width,int Height) GetFillSize() {
            return (parent.Width, parent.Height);
        }

        protected virtual (int Width,int Height) GetBoxFill() {
            int size = parent.Width > parent.Height ? parent.Height : parent.Width;
            return (size, size);
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
