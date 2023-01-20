using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TwelveEngine;
using TwelveEngine.Game3D.Entity.Types;

namespace Elves.Scenes.SaveSelect {

    public class SaveTag:Screenspace3DSprite {

        private static readonly Vector3 DefaultRotation = new(0,0,-2.25f);

        public static readonly Vector2 BaseSize = new(128,32);

        private static readonly Dictionary<TagState,Rectangle> textureSources = new() {
            { TagState.NoSave, new(1,50,128,32) },
            { TagState.CreateNew, new(52,126,128,32) },
            { TagState.Delete, new(52,160,128,32) },
            { TagState.Customized, new(1,90,128,32) },
        };

        private readonly int _saveID;
        public int SaveID => _saveID;

        public SaveTag(int ID,Texture2D texture):base(texture) {
            PixelSmoothing = false;
            _saveID = ID;
            OnUpdate += SaveTag_OnUpdate;
            animator.Finish();
            OnLoad += SaveTag_OnLoad;
        }

        private void SaveTag_OnLoad() {
            if(SaveID == 1) {
                shiftingLeft = true;
            }
        }

        public Vector2 Origin { get; set; } = Vector2.Zero;
        public float RenderScale { get; set; } = 1f;

        private readonly AnimationInterpolator animator = new(Constants.AnimationTiming.SaveTagHighlight);

        public bool shiftingLeft = false;

        public void ShiftLeft() {
            if(shiftingLeft) {
                return;
            }
            animator.ResetCarryOver(Now);
            shiftingLeft = true;
        }

        public void ShiftRight() {
            if(!shiftingLeft) {
                return;
            }
            animator.ResetCarryOver(Now);
            shiftingLeft = false;
        }

        private float GetShiftT() {
            if(shiftingLeft) {
                return animator.Value;
            } else {
                return 1 - animator.Value;
            }
        }

        public float LocalAnimationStrength { get; set; } = 1f;

        public TagState State { get; set; } = TagState.NoSave;

        private void SaveTag_OnUpdate() {
            animator.Update(Now);
            var scaledSize = RenderScale * BaseSize;
            var xOffset = GetShiftT() * scaledSize.X * 0.05f * LocalAnimationStrength;
            var position = Origin - scaledSize * 0.5f;
            position.X -= xOffset;
            Rotation = DefaultRotation * LocalAnimationStrength;
            Area = new VectorRectangle(position,scaledSize);

            TextureSource = textureSources.GetValueOrDefault(State);
        }

       public bool Contains(Point location) {
            return Area.Contains(location);
        }
    }
}
