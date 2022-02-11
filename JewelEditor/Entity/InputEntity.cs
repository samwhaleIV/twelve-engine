using System;
using System.Collections.Generic;
using TwelveEngine.Game2D;
using Microsoft.Xna.Framework;
using TwelveEngine.Input;
using JewelEditor.InputContext.MouseHandlers;
using Microsoft.Xna.Framework.Input;
using JewelEditor.HistoryActions;

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
            switch(GetState().InputMode) {
                default:
                case InputMode.Tile:
                    return tileEditor;
                case InputMode.Entity:
                    return entityEditor;
            }
        }

        private void ChangeInput(InputMode inputMode) {
            var state = GetState();
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
            if(!GetUI().TryGetInputMode(index,out var inputMode)) {
                return;
            }
            if(inputMode.Type.HasValue) {
                GetState().TileType = inputMode.Type.Value;
            }
            ChangeInput(inputMode.Mode);
        }

        private bool ControlKeyDown => Game.KeyboardState.IsKeyDown(Keys.LeftControl);

        private void AddNumberKeys(Queue<(Keys,Action)> set) {
            var numberKeys = new Keys[] {
                Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5,
                Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0
            };

            for(int i = 0;i<numberKeys.Length;i++) {
                Func<int,Action> actionGenerator = (int value) => {
                    return () => SetInput(value);
                };
                set.Enqueue((numberKeys[i],actionGenerator.Invoke(i)));
            }
        }

        private void QueueCardinalPoints(Queue<Point> queue,Point origin) {
            Point N = origin + new Point(0,-1),
                  E = origin + new Point(-1,0),
                  S = origin + new Point(0,1),
                  w = origin + new Point(1,0);
            if(Owner.TileInRange(N)) queue.Enqueue(N);
            if(Owner.TileInRange(E)) queue.Enqueue(E);
            if(Owner.TileInRange(S)) queue.Enqueue(S);
            if(Owner.TileInRange(w)) queue.Enqueue(w);
        }

        private void FillArea() {
            var state = GetState();
            if(state.InputMode != InputMode.Tile) return;
            Point point = Game.MouseState.Position;

            Owner.UpdateScreenSpace();
            if(!Owner.TryGetTile(point,out var tile)) return;

            var layer = Owner.GetLayer(Editor.TileLayer);
            var newValue = (int)state.TileType;

            var startValue = layer[tile.X,tile.Y];
            if(startValue == newValue) return;

            var eventToken = state.StartHistoryEvent();

            var queue = new Queue<Point>();
            queue.Enqueue(tile);

            int value;
            do {
                tile = queue.Dequeue();
                value = layer[tile.X,tile.Y];
                if(value != startValue) {
                    continue;
                }
                state.AddEventAction(eventToken,new SetTile(tile,newValue,value));

                QueueCardinalPoints(queue,origin: tile);
            } while(queue.Count > 0);

            state.EndHistoryEvent(eventToken);
        }

        private void InitializeKeyWatchers(Queue<(Keys,Action)> set) {
            AddNumberKeys(set);

            set.Enqueue((Keys.E, () => SetInput(Editor.EraserIndex)));

            set.Enqueue((Keys.Z, () => {if (ControlKeyDown) GetState().Undo(); }));
            set.Enqueue((Keys.Y, () => {if (ControlKeyDown) GetState().Redo(); }));

            set.Enqueue((Keys.F, () => FillArea()));
        }

        public InputEntity() {
            OnLoad += InputEntity_OnLoad;
            OnUnload += InputEntity_OnUnload;

            var keyWatchers = new Queue<(Keys,Action)>();
            InitializeKeyWatchers(keyWatchers);
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
            var UI = GetUI();
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
