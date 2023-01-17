using Microsoft.Xna.Framework;
using Elves.Scenes.Battle.Sprite.Animation;
using System;

namespace Elves.Scenes.Battle.Sprite.Elves {
    public sealed class HarmlessElf:BattleSprite {
        public HarmlessElf() : base("Elves/harmless-elf",47,
            AnimationFactory.CreateStatic(0,0,17,47),
            AnimationFactory.CreateIdleBlink(0,0,17,47,17,0,17,47),

            AnimationFactory.CreateSlideshowAndBack(
                AnimationType.Hurt,
                AnimationMode.Once,
                new Rectangle(0,47,17,47),3,
                TimeSpan.FromMilliseconds(50)
            ),

            AnimationFactory.CreateDead(34,0,17,47)
        ) {
            UserData.Color = Color.Red;
            UserData.Name = "Harmless Elf";
        }
    }
}
