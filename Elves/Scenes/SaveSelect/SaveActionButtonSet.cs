using Elves.Scenes.Battle.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine;

namespace Elves.Scenes.SaveSelect {


    public sealed class SaveActionButtonSet {
        private readonly Texture2D texture;

        public readonly List<SaveActionButton> Buttons = new();

        public SaveActionButtonSet(Texture2D texture) {
            this.texture = texture;
        }

        public void ClearButtons() {
            Buttons.Clear();
        }

        public int AddButton(SaveButtonType type) {
            int index = Buttons.Count;
            SaveActionButton button = new() {
                Texture = texture,
                Type = type
            };
            Buttons.Add(button);
            return index;
        }

        public void Update(float height,Rectangle targetArea) {
            if(Buttons.Count < 1) {
                return;
            }
            VectorRectangle bounds = new(targetArea);

            float centerX = bounds.Width * 0.5f + height * 0.5f;
            float buttonY = bounds.Height * (5 / 7f);
            float centerOffset = height * 0.05f;
            float totalWidth = height * Buttons.Count + centerOffset * (Buttons.Count - 1);
            float xStart = centerX - totalWidth * 0.5f;

            for(int i = 0;i<Buttons.Count;i++) {
                var button = Buttons[i];
                button.Scale = Scale;
                button.Update(new Vector2(xStart + (centerOffset + height) * i,buttonY),height);
            }
        }

        public float Scale { get; set; } = 1f;

        public void Render(SpriteBatch spriteBatch) {
            if(Buttons.Count < 1) {
                return;
            }
            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            foreach(var button in Buttons) {
                button.Render(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
