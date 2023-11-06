﻿using System;
using System.Collections.Generic;
using Elves.Animation;
using Elves.Battles;
using Microsoft.Xna.Framework;
using static Elves.Animation.AnimationFactory;

namespace Elves.ElfData {
    public static partial class ElfManifest {
        private static readonly Dictionary<ElfID,Elf> manifest = new();

        public static Elf Get(ElfID ID) => manifest[ID];
        public static void Add(Elf elf) => manifest.Add(elf.ID,elf);

        public static IEnumerable<Elf> GetAll() => manifest.Values;

        private static int _IDCounter = 0;

        private static ElfID GetAutoID() {
            int ID = _IDCounter;
            _IDCounter++;
            return (ElfID)ID;
        }

        private static ElfID AutoID => GetAutoID();
        public static int Count => manifest.Count;

        /* This should probably be information loaded from a data file, but C# is so expressive... I can't help myself. */
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
        }
    }
}
