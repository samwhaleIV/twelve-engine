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

        private IMouseTarget target = null;

        private IMouseTarget GetTarget(Point point) {
            var entity = Owner.Entities.Get<UIEntity>("UIEntity");
            if(entity != null && entity.Contains(Owner.GetWorldVector(point))) {
                return entity.MouseTarget;
            } else {
                return null;
            }
        }

        private void MouseHandler_OnMouseScroll(Point point,ScrollDirection direction) {
            if(target == null) {
                GetTarget(point)?.Scroll(point,direction);
            } else {
                target.Scroll(point,direction);
            }
        }

        private void MouseHandler_OnMouseMove(Point point) {
            if(target == null) {
                GetTarget(point)?.MouseMove(point);
            } else {
                target.MouseMove(point);
            }
        }

        private void MouseHandler_OnMouseUp(Point point) {
            if(target == null) {
                return;
            }
            target.MouseUp(point);
            target = null;
        }

        private void MouseHandler_OnMouseDown(Point point) {
            target = GetTarget(point);
            if(target == null) {
                target = mouseController;
            }
            target.MouseDown(point);
        }
    }
}
