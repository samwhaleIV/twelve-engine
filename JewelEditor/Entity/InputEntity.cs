using TwelveEngine.Game2D;
using Microsoft.Xna.Framework;
using TwelveEngine.Input;
using JewelEditor.InputContext.MouseHandlers;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace JewelEditor.Entity {
    internal sealed class InputEntity:JewelEntity {
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
            if(Owner.Game.KeyboardState.IsKeyDown(Keys.LeftShift)) {
                return pointer;
            }
            switch(State.InputMode) {
                default:
                case InputMode.Tile:
                    return tileEditor;
                case InputMode.Entity:
                    return entityEditor;
            }
        }

        private void ChangeInput(InputMode inputMode) {
            var state = State;
            var oldInput = state.InputMode;

            if(oldInput == inputMode) {
                return;
            }

            state.InputMode = inputMode;
            if(target == null) {
                return;
            }
            var point = Game.MouseHandler.Position;
            target.MouseUp(point);
            target = null;
        }

        private readonly KeyWatcherSet keyWatcherSet;

        private void SetInput(int index) {
            if(!UI.TryGetInputMode(index,out var inputMode)) {
                return;
            }
            if(inputMode.Type.HasValue) {
                State.TileType = inputMode.Type.Value;
            }
            ChangeInput(inputMode.Mode);
        }

        public InputEntity() {
            OnLoad += InputEntity_OnLoad;
            OnUnload += InputEntity_OnUnload;

            var numberKeys = new Keys[] {
                Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5,
                Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0
            };

            var keyWatchers = new Queue<(Keys,Action)>();

            for(int i = 0;i<numberKeys.Length;i++) {
                Func<int,Action> actionGenerator = (int value) => {
                    return () => SetInput(value);
                };
                keyWatchers.Enqueue((numberKeys[i],actionGenerator.Invoke(i)));
            }

            keyWatchers.Enqueue((Keys.E,()=>SetInput(3)));

            keyWatcherSet = new KeyWatcherSet(keyWatchers.ToArray());
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

        private IMouseTarget GetTarget(Point point,bool dropFocus = false) {
            bool inUIArea = UI.Contains(Owner.GetWorldVector(point));
            if(!inUIArea) {
                if(dropFocus) {
                    UI.DropFocus();
                }
                return null;
            }

            return UI.MouseTarget;
        }

        private void MouseHandler_OnMouseScroll(Point point,ScrollDirection direction) {
            var target = GetTarget(point);
            if(target == null) {
                target = pointer;
            }
            target.Scroll(point,direction);
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
