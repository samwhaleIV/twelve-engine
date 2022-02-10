using TwelveEngine.Game2D;
using Microsoft.Xna.Framework;
using TwelveEngine.Input;
using JewelEditor.InputContext.MouseHandlers;
using Microsoft.Xna.Framework.Input;

namespace JewelEditor.InputContext {
    internal sealed class InputEntity:Entity2D {
        protected override int GetEntityType() => JewelEntities.InputEntity;

        private Pointer pointer;
        private EntityEditor entityEditor;
        private TileEditor tileEditor;

        private void LoadMouseHandlers(Grid2D grid) {
            pointer = new Pointer(grid);
            entityEditor = new EntityEditor(grid);
            tileEditor = new TileEditor(grid);
        }

        private IMouseTarget GetMouseHandlerTarget() {
            var entity = GetUIEntity();
            switch(entity.InputMode) {
                default:
                case InputMode.Pointer:
                    return pointer;
                case InputMode.Entity:
                    return entityEditor;
                case InputMode.NoTile:
                case InputMode.FloorTile:
                case InputMode.WallTile:
                case InputMode.DoorTile:
                    return tileEditor;
            }
        }

        private void ChangeInput(InputMode inputMode) {
            var entity = GetUIEntity();
            entity.InputMode = inputMode;
            if(target == null) {
                return;
            }
            var point = Game.MouseHandler.Position;
            target.MouseUp(point);
            target = null;
        }

        private readonly KeyWatcherSet keyWatcherSet;

        public InputEntity() {
            OnLoad += InputEntity_OnLoad;
            OnUnload += InputEntity_OnUnload;
            keyWatcherSet = new KeyWatcherSet(
                (Keys.D1, () => ChangeInput(InputMode.Pointer)),
                (Keys.D2, () => ChangeInput(InputMode.Entity)),
                (Keys.D3, () => ChangeInput(InputMode.NoTile)),
                (Keys.D4,() => ChangeInput(InputMode.FloorTile)),
                (Keys.D5,() => ChangeInput(InputMode.WallTile)),
                (Keys.D6,() => ChangeInput(InputMode.DoorTile))
            );
        }

        private void InputEntity_OnLoad() {

            LoadMouseHandlers(Owner);

            var mouseHandler = Game.MouseHandler;
            mouseHandler.OnMouseDown += MouseHandler_OnMouseDown;
            mouseHandler.OnMouseUp += MouseHandler_OnMouseUp;
            mouseHandler.OnMouseMove += MouseHandler_OnMouseMove;
            mouseHandler.OnMouseScroll += MouseHandler_OnMouseScroll;
            OnUpdate += InputEntity_OnUpdate;
        }

        private void InputEntity_OnUpdate(GameTime gameTime) {
            keyWatcherSet.Update(Game.KeyboardState);
        }

        private void InputEntity_OnUnload() {
            var mouseHandler = Game.MouseHandler;
            mouseHandler.OnMouseDown -= MouseHandler_OnMouseDown;
            mouseHandler.OnMouseUp -= MouseHandler_OnMouseUp;
            mouseHandler.OnMouseMove -= MouseHandler_OnMouseMove;
            mouseHandler.OnMouseScroll -= MouseHandler_OnMouseScroll;
        }

        private IMouseTarget target = null;

        private UIEntity GetUIEntity() => Owner.Entities.Get<UIEntity>("UIEntity");

        private IMouseTarget GetTarget(Point point,bool dropFocus = false) {
            var entity = GetUIEntity();

            bool inUIArea = entity.Contains(Owner.GetWorldVector(point));
            if(!inUIArea) {
                if(dropFocus) {
                    entity.DropFocus();
                }
                return null;
            }

            return entity.MouseTarget;
        }

        private void MouseHandler_OnMouseScroll(Point point,ScrollDirection direction) {
            if(target == null) {
                var tempTarget = GetTarget(point);
                if(tempTarget == null) {
                    tempTarget = GetMouseHandlerTarget();
                }
                tempTarget.Scroll(point,direction);
            } else {
                target.Scroll(point,direction);
            }
        }

        private void MouseHandler_OnMouseMove(Point point) {
            if(target == null) {
                GetTarget(point,dropFocus: true)?.MouseMove(point);
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
                target = GetMouseHandlerTarget();
            }
            target.MouseDown(point);
        }
    }
}
