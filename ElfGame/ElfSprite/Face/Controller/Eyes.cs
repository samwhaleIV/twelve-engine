using System;

namespace ElfGame.ElfSprite.Face.Controller {
    internal sealed class Eyes:Component {

        private static TimeSpan getTime(double value) => TimeSpan.FromMilliseconds(value);
            private TimeSpan blinkDelay = getTime(Constants.ElfBlinkDelay);
            private TimeSpan blinkDuration = getTime(Constants.ElfBlinkDuration);
            private TimeSpan winkDuration = getTime(Constants.ElfWinkDuration);

        private void blinkStart() {
            State.EyeBlink = EyeBlink.Blink;
            SetTimeout(blinkEnd,blinkDuration);
        }

        private void blinkEnd() {
            State.EyeBlink = EyeBlink.None;
            SetTimeout(blinkStart,blinkDelay);
        }

        public bool IsBlinking { get; private set; }

        public void ToggleBlinking() {
            if(IsBlinking) {
                StopBlinking();
            } else {
                StartBlinking();
            }
        }

        public void StopBlinking() {
            if(!IsBlinking) {
                return;
            }
            IsBlinking = false;
            ClearTimeout();
        }

        public void StartBlinking() {
            if(IsBlinking) {
                return;
            }
            IsBlinking = true;
            blinkStart();
        }

        private void winkEnd() {
            State.EyeBlink = EyeBlink.None;
            if(IsBlinking) blinkEnd();
        }

        private void winkStart(EyeBlink type) {
            State.EyeBlink = type;
            SetTimeout(winkEnd,winkDuration);
        }

        public void LeftWink() => winkStart(EyeBlink.LeftWink);
        public void RightWink() => winkStart(EyeBlink.RightWink);
    }
}
