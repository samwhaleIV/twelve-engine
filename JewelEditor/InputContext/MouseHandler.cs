using JewelEditor.Entity;
using Microsoft.Xna.Framework;
using System;
using TwelveEngine.Game2D;
using TwelveEngine.Input;

namespace JewelEditor.InputContext {
    internal abstract class MouseHandler:IMouseTarget {

        private readonly Grid2D _grid;
        public MouseHandler(Grid2D grid) => _grid = grid;

        protected Grid2D Grid => _grid;
        protected StateEntity State => Grid.Entities.Get<StateEntity>(Editor.State);

        protected Vector2 TranslatePoint(Point point) => Grid.GetWorldVector(point);

        public abstract void MouseDown(Point point);
        public abstract void MouseMove(Point point);
        public abstract void MouseUp(Point point);

        protected event Action<Point,ScrollDirection> OnScroll;
        public void Scroll(Point point,ScrollDirection direction) => OnScroll?.Invoke(point,direction);
    }
}
