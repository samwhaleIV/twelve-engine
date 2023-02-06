using System;
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

        /* This should probably be information loaded from a data file, but C# is so expressive... I can't help myself. */
        static ElfManifest() {
            Add(Elf.Create<DebugBattle>(
                ID: ElfID.HarmlessElf,
                name: "Harmless Elf",
                texture: "Elves/harmless-elf",
                color: Color.Red,
                baseHeight: 47,
                frameSets: new FrameSet[] { CreateStatic(0,0,19,47) }
            ));
            Add(Elf.Create<DebugBattle>(
                ID: ElfID.NinjaElf,
                name: "Harmless Elf",
                texture: "Elves/harmless-elf",
                color: Color.Red,
                baseHeight: 47,
                frameSets: new FrameSet[] { CreateStatic(0,0,19,47) }
            ));
            Add(Elf.Create<DebugBattle>(
                ID: ElfID.GodElf,
                name: "Harmless Elf",
                texture: "Elves/harmless-elf",
                color: Color.Red,
                baseHeight: 47,
                frameSets: new FrameSet[] { CreateStatic(0,0,19,47) }
            ));
            Add(Elf.Create<DebugBattle>(
                ID: ElfID.BusinessElf,
                name: "Harmless Elf",
                texture: "Elves/harmless-elf",
                color: Color.Red,
                baseHeight: 47,
                frameSets: new FrameSet[] { CreateStatic(0,0,19,47) }
            ));
        }
    }
}
