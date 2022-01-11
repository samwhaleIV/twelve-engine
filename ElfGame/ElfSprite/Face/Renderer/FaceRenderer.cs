using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ElfGame.ElfSprite.Face {
    internal sealed class FaceRenderer {

        private readonly FaceSources sources = new FaceSources();
        private readonly FaceTargets targets = new FaceTargets();

        public FaceState State { get; set; }
        public Texture2D Texture { get; set; }

        public Point Size => sources.FaceBackground.Size;

        private SpriteBatch spriteBatch;
        private Point renderLocation;

        private void draw(Rectangle target,Rectangle source) {
            var location = target.Location + renderLocation;
            var size = target.Size;
            var destination = new Rectangle(location,size);
            spriteBatch.Draw(Texture,destination,source,Color.White);
        }

        private void drawBruise(Rectangle target) => draw(target,sources.EyeBruise);

        private void drawLeftBruise() => drawBruise(targets.LeftBruise);
        private void drawRightBruise() => drawBruise(targets.RightBruise);

        private void drawLeftTears() => draw(targets.LeftTear,sources.LeftEyeTears);
        private void drawRightTears() => draw(targets.RightTear,sources.RightEyeTears);

        private bool hasEyes() => State.EyeType != EyeType.None;

        private (bool,bool) getBruising() {
            var bruising = State.EyeBruising;

            if(bruising == EyeBruising.None) return (false, false);
            if(bruising == EyeBruising.All) return (true, true);

            return (bruising == EyeBruising.Left, bruising == EyeBruising.Right);
        }

        private (bool,bool) getTearing() {
            var tears = State.EyeTears;

            if(tears == EyeTears.None) return (false, false);
            if(tears == EyeTears.All) return (true, true);

            return (tears == EyeTears.Left, tears == EyeTears.Right);
        }

        private (bool,bool) getBlinking() {
            var blink = State.EyeBlink;

            if(blink == EyeBlink.None) return (false, false);
            if(blink == EyeBlink.Blink) return (true, true);

            return (blink == EyeBlink.LeftWink, blink == EyeBlink.RightWink);
        }

        private void drawEyeDecoration(
            (bool Left, bool Right) tearing,
            (bool Left, bool Right) bruising
        ) {
            if(tearing.Left) {
                if(hasEyes()) drawLeftTears();
            } else if(bruising.Left) {
                drawLeftBruise();
            }

            if(tearing.Right) {
                if(hasEyes()) drawRightTears();
            } else if(bruising.Right) {
                drawRightBruise();
            }
        }

        private void drawBackground() {
            var destination = new Rectangle(renderLocation,sources.FaceBackground.Size);
            spriteBatch.Draw(Texture,destination,sources.FaceBackground,Color.White);
        }

        private Rectangle getEyesSource() {
            switch(State.EyeType) {
                default:
                case EyeType.Normal: return sources.EyesNormal;
                case EyeType.Sly: return sources.EyesSly;
                case EyeType.Uncaring: return sources.EyesUncaring;
                case EyeType.Dazed: return sources.EyesDazed;
            }
        }

        private Rectangle getEyelidSource(bool isBruised) {
            return isBruised ? sources.BruisedEyelid : sources.NormalEyelid;
        }

        private void drawBlinking((bool Left, bool Right) bruising) {
            (bool Left, bool Right) blinking = getBlinking();
            if(blinking.Left) {
                draw(targets.LeftEyelid,getEyelidSource(bruising.Left));
            }
            if(blinking.Right) {
                draw(targets.RightEyelid,getEyelidSource(bruising.Right));
            }
        }

        private void drawEyes() {
            (bool Left, bool Right) tearing = getTearing(), bruising = getBruising();
            drawEyeDecoration(tearing,bruising);
            if(!hasEyes()) {
                return;
            }
            draw(targets.Eyes,getEyesSource());
            drawBlinking(bruising);
        }

        private Rectangle getNormalMouthSource() {
            return State.MouthMode == MouthMode.Talk ? sources.MouthOpen : sources.MouthClosed;
        }

        private void drawMouth() {
            Rectangle source;
            switch(State.MouthType) {
                case MouthType.None:
                    return;
                default:
                case MouthType.Normal:
                    source = getNormalMouthSource();
                    break;
                case MouthType.Scream:
                    source = sources.MouthScream;
                    break;
                case MouthType.Sad:
                    source = sources.MouthSad;
                    break;
            }
            draw(targets.Mouth,source);
        }

        public void Render(SpriteBatch spriteBatch,Point renderLocation) {
           
            if(Texture == null) {
                return;
            }

            this.spriteBatch = spriteBatch;
            this.renderLocation = renderLocation;

            drawBackground();
            drawEyes();
            drawMouth();

            this.spriteBatch = null;
        }
        
    }
}
