﻿using System;
using System.Collections.Generic;
using TwelveEngine.Game2D;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game2D.Entity;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TwelveEngine.Serial;
using System.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using TwelveEngine.Game2D.Objects;

namespace EntropyGame.States {
    internal sealed class TestWorld:PhysicsGrid2D {

        private struct Line {
            public Vector2 A, B;
            public Line(Vector2 a,Vector2 b) { A = a; B = b; }
        }

        private const int PLAYER_TYPE = 1;

        private const int LINE_TYPE = 2;

        private sealed class LineEntity:Entity2D {

            public Line Line { get; set; }
            public Color Color { get; set; } = Color.Red;

            public LineEntity() {
                OnImport += LineEntity_OnImport;
                OnExport += LineEntity_OnExport;
                OnRender += LineEntity_OnRender;
                OnLoad += LineEntity_OnLoad;
                OnUnload += LineEntity_OnUnload;
            }

            private World PhysicsWorld => (Owner as PhysicsGrid2D).PhysicsWorld;

            private Body body;

            private void LineEntity_OnLoad() {

                body = PhysicsWorld.CreateEdge(Line.A,Line.B);
            }

            private void LineEntity_OnUnload() {
                PhysicsWorld.Remove(body);
            }

            /* https://community.monogame.net/t/line-drawing/6962/5 */

            private void DrawLine(Vector2 point1,Vector2 point2,float thickness = 1f) {
                var distance = Vector2.Distance(point1,point2);
                var angle = (float)Math.Atan2(point2.Y - point1.Y,point2.X - point1.X);
                DrawLine(point1,distance,angle,thickness);
            }

            private void DrawLine(Vector2 point,float length,float angle,float thickness) {
                var origin = new Vector2(0f,0.5f);
                var scale = new Vector2(length,thickness);
                Owner.Game.SpriteBatch.Draw(Owner.BlankTexture,point,null,Color,angle,origin,scale,SpriteEffects.None,0f);
            }

            private void LineEntity_OnRender(GameTime gameTime) {
                Point screenA = Owner.GetScreenPoint(Line.A), screenB = Owner.GetScreenPoint(Line.B);
                DrawLine(screenA.ToVector2(),screenB.ToVector2(),1f);
            }

            private void LineEntity_OnExport(SerialFrame frame) {
                frame.Set(Line.A);
                frame.Set(Line.B);
                frame.Set(Color);
            }

            private void LineEntity_OnImport(SerialFrame frame) {
                Vector2 a = frame.GetVector2();
                Vector2 b = frame.GetVector2();
                Line = new Line(a,b);
                Color = frame.GetColor();
            }

            protected override int GetEntityType() => LINE_TYPE;
        }

        private sealed class Player:TopDownPlayer {

            private Texture2D texture;

            public Player() {
                CameraTracking = true;
                OnRender += Player_OnRender;
                OnLoad += Player_OnLoad;
            }

            protected override int GetEntityType() => PLAYER_TYPE;

            private void Player_OnLoad() {
                texture = Game.Content.Load<Texture2D>("testing-testing-cat");
            }

            private void Player_OnRender(GameTime gameTime) {
                if(!OnScreen()) {
                    return;
                }
                Draw(texture);
            }
        }

        private void World_OnLoad() {
            var player = Entities.Create(PLAYER_TYPE,"test-player");
            player.Position = new Vector2(2.5f,2.5f);
        }

        public TestWorld() {
            OnLoad += World_OnLoad;
            UnitSize = 16;
            EntityFactory = new EntityFactory<Entity2D,Grid2D>(
                (PLAYER_TYPE, () => new Player()),
                (LINE_TYPE, () => new LineEntity())
            );
            Mouse.OnPress += AddPoint;
            Mouse.OnRelease += AddPoint;
            BackgroundColor = Color.LightGray;

            SetAtlas("tileset");
            PhysicsGameObject gameObject = Objects.CreateKinematic();
            gameObject.AngularVelocity = 1f;

            gameObject.Position = new Vector2(2,2);
            gameObject.Size = new Vector2(1,2.25F);
            gameObject.TextureSource = new Rectangle(16,0,16,16);

            gameObject.HitboxSize = new Vector2(1,0.5F);
            gameObject.HitboxOffset = new Vector2(0.25f,0.125F);

            RenderObjectHitboxes = true;
        }

        private List<Line> lines = new List<Line>();

        private Vector2? lineStart = null;

        private readonly Color[] colors = new Color[] {
            Color.Red, Color.Blue, Color.Green,
            Color.Orange, Color.Yellow, Color.Purple, Color.Pink
        };

        private int colorIndex = 0;

        private void AddPoint(float x,float y) {
            AddPoint(new Vector2(x,y));
        }

        private void AddPoint(Vector2 worldPoint) {

            if(!lineStart.HasValue) {
                lineStart = worldPoint;
                return;
            }

            Line line = new Line(lineStart.Value,worldPoint);
            lineStart = null;

            var lineEntity = new LineEntity() {
                Line = line,
                Color = colors[colorIndex = (colorIndex + 1) % colors.Length],
            };
            Entities.Add(lineEntity);

            lines.Add(line);
        }

        private void AddPoint(Point point) {
            Vector2 worldPoint = GetWorldVector(point);
            Debug.WriteLine($"AddPoint({worldPoint.X}f,{worldPoint.Y}f);");
            AddPoint(worldPoint);
        }
    }
}
