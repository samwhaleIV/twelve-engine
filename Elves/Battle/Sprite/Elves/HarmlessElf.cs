using Microsoft.Xna.Framework;
using Elves.Battle.Sprite.Animation;

namespace Elves.Battle.Sprite.Elves {
    public sealed class HarmlessElf:BattleSprite {

        private const int BASE_HEIGHT = 47;

        public HarmlessElf(int baseHeight = BASE_HEIGHT) : base("Elves/harmless-elf",new FrameSet[] {
            AnimationFactory.CreateStatic(0,0,17,47),
            AnimationFactory.CreateIdleBlink(0,0,17,47,17,0,17,47)
        },baseHeight) {
            AccentColor = Color.Red;
        }
    }
}
