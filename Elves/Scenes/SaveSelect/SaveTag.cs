using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine;
using TwelveEngine.Game3D.Entity.Types;

namespace Elves.Scenes.SaveSelect {
    public class SaveTag:Screenspace3DSprite {

        public static readonly Vector2 BaseSize = new(128,32);

        private static readonly Rectangle DefaultArea = new(1,90,(int) BaseSize.X,(int) BaseSize.Y);
        private static readonly Rectangle NoSaveArea = new(1,50,(int)BaseSize.X,(int)BaseSize.Y);

        private void SetDefaultTextureSource() => TextureSource = DefaultArea;
        private void SetNoSaveTextureSource() => TextureSource = NoSaveArea;

        private readonly int _saveID;
        public int SaveID => _saveID;

        public SaveTag(int ID,Texture2D texture):base(texture) {
            PixelSmoothing = false;
            _saveID = ID;
            OnUpdate += SaveTag_OnUpdate;
            SetNoSaveTextureSource();
            animator.Finish();
            OnLoad += SaveTag_OnLoad;
        }

        private void SaveTag_OnLoad() {
            if(SaveID == 1) {
                shiftingLeft = true;
            }
        }

        public Vector2 Origin { get; set; } = Vector2.Zero;
        public float TagScale { get; set; } = 1f;

        private readonly AnimationInterpolator animator = new(Constants.AnimationTiming.SaveFingerMovementDuration);

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
                return animator.GetValue();
            } else {
                return 1 - animator.GetValue();
            }
        }

        private void SaveTag_OnUpdate() {
            animator.Update(Now);
            var scaledSize = TagScale * BaseSize;
            var xOffset = GetShiftT() * scaledSize.X * 0.05f;
            var position = Origin - scaledSize * 0.5f;
            position.X -= xOffset;
            Area = new VectorRectangle(position,scaledSize);
        }

        private bool _empty = true;

        public bool Empty {
            get => _empty;
            set {
                if(value == _empty) {
                    return;
                }
                if(value) {
                    SetNoSaveTextureSource();
                } else {
                    SetDefaultTextureSource();
                }
            }
        }
    }
}
