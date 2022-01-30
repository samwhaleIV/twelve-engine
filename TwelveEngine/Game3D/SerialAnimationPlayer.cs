using TwelveEngine.Serial;
using Liru3D.Animations;
using Liru3D.Models;

namespace TwelveEngine.Game3D {
    public sealed class SerialAnimationPlayer:AnimationPlayer, ISerializable {

        internal SerialAnimationPlayer(SkinnedModel model) : base(model) {
            Animation = model.Animations[0];
            animationIndex = 0;
            IsLooping = false;
        }

        private const int UNKNOWN_INDEX = -1;

        private int animationIndex = UNKNOWN_INDEX;

        public string AnimationName {
            get => Animation?.Name;
            set => SetAnimationName(value);
        }

        private void SetAnimationName(string value) {
            var animationCount = Model.AnimationCount;
            for(int i = 0;i<animationCount;i++) {
                var animation = Model.Animations[i];
                if(animation.Name != value) {
                    continue;
                }
                Animation = animation;
                animationIndex = AnimationIndex;
                return;
            }
        }

        private int GetAnimationIndex() {
            if(animationIndex >= 0) {
                return animationIndex;
            }
            var animationName = AnimationName;
            if(string.IsNullOrEmpty(animationName)) {
                return UNKNOWN_INDEX;
            }
            var animationCount = Model.AnimationCount;
            for(int i = 0;i<animationCount;i++) {
                var animation = Model.Animations[i];
                if(animationName != animation.Name) {
                    continue;
                }
                animationIndex = i;
                return i;
            }
            return UNKNOWN_INDEX;
        }

        public int AnimationIndex {
            set {
                animationIndex = value % Model.AnimationCount;
                Animation = Model.Animations[animationIndex];
            }
            get => GetAnimationIndex();
        }

        public int AnimationCount => Model.AnimationCount;

        public void Import(SerialFrame frame) {
            IsPlaying = frame.GetBool();
            IsLooping = frame.GetBool();

            CurrentTime = frame.GetFloat();
            PlaybackSpeed = frame.GetFloat();

            AnimationName = frame.GetString();
        }

        public void Export(SerialFrame frame) {
            frame.Set(IsPlaying);
            frame.Set(IsLooping);

            frame.Set(CurrentTime);
            frame.Set(PlaybackSpeed);

            frame.Set(AnimationName);
        }

    }
}
