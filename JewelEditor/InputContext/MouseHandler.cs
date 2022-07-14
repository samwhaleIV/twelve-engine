using JewelEditor.Entity;
using Microsoft.Xna.Framework;
using System;
using TwelveEngine.Game2D;
using TwelveEngine.Shell.Input;

namespace JewelEditor.InputContext {
    internal abstract class MouseHandler:IMouseTarget {

        private readonly TileGrid _grid;
        public MouseHandler(TileGrid grid) => _grid = grid;

        protected TileGrid Grid => _grid;
        protected StateEntity GetState() => Grid.Entities.Get<StateEntity>(Editor.State);

        public abstract void MouseDown(Point point);
        public abstract void MouseMove(Point point);
        public abstract void MouseUp(Point point);

        protected event Action<Point,ScrollDirection> OnScroll;
        public void Scroll(Point point,ScrollDirection direction) => OnScroll?.Invoke(point,direction);
    }
}
