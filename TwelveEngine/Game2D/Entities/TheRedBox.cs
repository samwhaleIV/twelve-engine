using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D.Entities {
    class TheRedBox:Entity,IRenderable,IUpdateable  {

        float X { get; set; } = 0;
        float Y { get; set; } = 0;

        public override void Export(SerialFrame frame) {
            frame.Set("X",X);
            frame.Set("Y",Y);
        }

        public override void Import(SerialFrame frame) {
            X = frame.GetFloat("X");
            Y = frame.GetFloat("Y");
        }

        private Texture2D redBoxTexture;

        public override void Load() {
            FactoryID = "TheRedBox";
            redBoxTexture = new Texture2D(Game.GraphicsDevice,1,1);
            redBoxTexture.SetData(new Color[] { Color.Red });
        }

        public void Render(GameTime gameTime) {
            Game.SpriteBatch.Draw(redBoxTexture,new Rectangle(0,0,16,16),Color.White);
        }

        public override void Unload() {
            redBoxTexture.Dispose();
        }

        public void Update(GameTime gameTime) {}
    }
}
