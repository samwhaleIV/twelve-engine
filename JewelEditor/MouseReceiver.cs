using TwelveEngine.Game2D;
using Microsoft.Xna.Framework;
using TwelveEngine.Input;

namespace JewelEditor {
    internal sealed class MouseReceiver:Entity2D {
        protected override int GetEntityType() => JewelEntities.MouseReceiver;

        private MouseController mouseController;

        public MouseReceiver() {
            OnLoad += MouseReceiver_OnLoad;
            OnUnload += MouseReceiver_OnUnload;
        }

        private void MouseReceiver_OnLoad() {
            var mouseHandler = Game.MouseHandler;
            mouseController = new MouseController(Owner);

            mouseHandler.OnMouseDown += MouseHandler_OnMouseDown;
            mouseHandler.OnMouseUp += MouseHandler_OnMouseUp;
            mouseHandler.OnMouseMove += MouseHandler_OnMouseMove;
            mouseHandler.OnMouseScroll += MouseHandler_OnMouseScroll;
        }

        private void MouseReceiver_OnUnload() {
            var mouseHandler = Game.MouseHandler;
            mouseHandler.OnMouseDown -= MouseHandler_OnMouseDown;
            mouseHandler.OnMouseUp -= MouseHandler_OnMouseUp;
            mouseHandler.OnMouseMove -= MouseHandler_OnMouseMove;
            mouseHandler.OnMouseScroll -= MouseHandler_OnMouseScroll;
        }

        private void MouseHandler_OnMouseScroll(Point point,ScrollDirection direction) {
            mouseController.Scroll(point,direction);
        }

        private void MouseHandler_OnMouseMove(Point point) {
            mouseController.MouseMove(point);
        }

        private void MouseHandler_OnMouseUp(Point point) {
            mouseController.MouseUp(point);
        }

        private void MouseHandler_OnMouseDown(Point point) {
            mouseController.MouseDown(point);
        }
    }
}
