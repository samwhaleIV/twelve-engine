using System.Collections.Generic;
using Elves.Animation;
using Elves.Battles;
using Microsoft.Xna.Framework;
using static Elves.Animation.AnimationFactory;

namespace Elves.ElfData {
    public static partial class ElfManifest {

        private static readonly SortedDictionary<int,Elf> manifest = new();
        public static void Add(Elf elf) => manifest.Add(elf.ID,elf);

        public static Elf Get(int ID) => manifest[ID];
        public static IEnumerable<Elf> GetAll() => manifest.Values;

        private static int _IDCounter = 0;
        private static int GetAutoID() => _IDCounter++;

        private static int AutoID => GetAutoID();
        public static int Count => manifest.Count;

        static ElfManifest() {
            Add(Elf.Create<RebootElf>(
                ID: AutoID,
                name: "Reboot Elf",
                texture: "Elves/reboot-elf",
                color: Color.Red,
                baseHeight: 47,
                frameSets: new FrameSet[] {
                    CreateStatic(0,0,19,47),
                    CreateIdleBlink(new(0,0,19,47),new(19,0,19,47)),
                    CreateDead(new(38,0,19,47)),
                    CreateSlideshowAndBack(AnimationType.Hurt,AnimationMode.Once,new(0,47,19,47),3),
                    FrameSet.CreateStatic(AnimationType.Backwards,new(57,0,19,47)),
                    FrameSet.CreateStatic(AnimationType.NoHead,new(76,0,19,47))
                }
            ));
            Add(Elf.Create<GreenElf>(
                ID: AutoID,
                name: "Green Elf",
                texture: "Elves/green-elf",
                color: Color.Green,
                baseHeight: 44,
                frameSets: new FrameSet[] {
                    CreateStatic(0,0,17,44),
                    CreateIdleBlink(new(0,0,17,44),new(17,0,17,44)),
                    CreateDead(new(34,0,17,44)),
                    CreateSlideshowAndBack(AnimationType.Hurt,AnimationMode.Once,new(0,44,17,44),3)
                }
            ));
        }
    }
}
