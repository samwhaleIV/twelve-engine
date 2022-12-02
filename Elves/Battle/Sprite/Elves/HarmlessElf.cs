using Microsoft.Xna.Framework;

namespace Elves.Battle.Sprite.Elves {
    public sealed class HarmlessElf:BattleSprite {

        public HarmlessElf(int baseHeight) : base("Elves/harmless-elf",null,baseHeight) {
            AccentColor = Color.Red;
        }
    }
}
