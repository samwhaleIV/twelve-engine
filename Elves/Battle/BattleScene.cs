using TwelveEngine.Shell.States;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine;
using TwelveEngine.Game3D;
using TwelveEngine.Game3D.Entity;
using TwelveEngine.Game3D.Entity.Types;
using TwelveEngine.Shell.UI;

namespace Elves.Battle {
    public class BattleScene:World {

        public double BackgroundScrollTime { get; set; } = 60d;

        public bool DebugCamera { get; set; } = false;

        public BattleScene(string backgroundImage) {
            OnLoad += () => {
                var camera = new AngleCamera() {
                    NearPlane = 0.1f,
                    FarPlane = 20f,
                    FieldOfView = 75f,
                    Orthographic = true,
                    Angle = new Vector2(0f,180f),
                    Position = new Vector3(0f,0f,10f)
                };
                Camera = camera;
                var backgroundEntity = new TextureEntity(backgroundImage) {
                    Name = "ScrollingBackground",
                    PixelSmoothing = true,
                    Billboard = true,
                    Scale = new Vector3(1f)
                };
                Entities.Add(backgroundEntity);

                var battleSprite = new BattleSprite("Elves/elf-sheet",3,9,17,47,52);
                battleSprite.Name = "BattleSprite";
                battleSprite.SpritePosition = SpritePosition.CenterLeft;
                Entities.Add(battleSprite);

                battleSprite = new BattleSprite("Elves/elf-sheet",22,4,24,52,52);
                battleSprite.SpritePosition = SpritePosition.CenterRight;
                Entities.Add(battleSprite);
            };
            OnUpdate += gameTime => {
                UpdateCamera();
                UpdateBackground(gameTime);
            };
            OnRender += gameTime => {
                RenderEntities(gameTime);
            };
            OnWriteDebug += writer => {
                if(!DebugCamera) {
                    return;
                }
                writer.ToTopLeft();
                writer.Write(Camera.Position);
                if(!(Camera is AngleCamera angleCamera)) {
                    return;
                }
                writer.WriteXY(angleCamera.Yaw,angleCamera.Pitch,"Yaw","Pitch");
            };
            bool transitionEnabled = true;
            int positionIndex = 0;
            SpritePosition[] positions = new SpritePosition[] {
                SpritePosition.Center,SpritePosition.Left,SpritePosition.Right,SpritePosition.CenterLeft,SpritePosition.CenterRight
            };
            Mouse.OnPress += point => {
                if(!transitionEnabled) {
                    return;
                }
                transitionEnabled = false;
                var battleSprite = Entities.Get<BattleSprite>("BattleSprite");
                battleSprite.SetSpritePosition(Game.Time,positions[positionIndex++%positions.Length],() => {
                    transitionEnabled = true;
                });
                return;
            };
        }

        private void UpdateBackground(GameTime gameTime) {
            TextureEntity background = Entities.Get<TextureEntity>("ScrollingBackground");
            background.SetColors(Color.Red,Color.Purple,Color.Red,Color.Purple);
            double scrollT = gameTime.TotalGameTime.TotalSeconds / BackgroundScrollTime % 1d;
            background.UVOffset = new Vector2((float)scrollT,0f);
        }

        private void UpdateCamera() {
            if(DebugCamera && Camera is AngleCamera angleCamera) {
                angleCamera.UpdateFreeCam(this,0.05f,0.01f);
            }
            Camera.Update(Game.Viewport.AspectRatio);
        }

        protected override void ResetGraphicsDeviceState(GraphicsDevice graphicsDevice) {
            graphicsDevice.Clear(Color.Black);
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        }
    }
}
