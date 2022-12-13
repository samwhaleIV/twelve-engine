namespace Elves.UI.Battle {
    public interface IBattleUIAnimated {

        protected bool GetAnimationCompleted();

        public bool AnimationCompleted => GetAnimationCompleted();

    }
}
