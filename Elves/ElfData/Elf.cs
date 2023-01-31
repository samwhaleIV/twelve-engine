using Elves.Animation;
using Elves.Battle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Elves.ElfData {
    public readonly struct Elf {

        public readonly Texture2D Texture;
        public readonly ElfID BattleID;
        public readonly string Name;
        public readonly int BaseHeight;
        public readonly FrameSet[] FrameSets;
        public readonly Color Color;
        public readonly Func<BattleScript> ScriptGenerator;

        private Elf(ElfID ID,string name,Func<BattleScript> scriptGenerator,string texture,Color color,int baseHeight,FrameSet[] frameSets) {
            ScriptGenerator = scriptGenerator;
            Texture = Program.Game.Content.Load<Texture2D>(texture);
            BattleID = ID;
            Name = name;
            BaseHeight = baseHeight;
            FrameSets = frameSets;
            Color = color;
        }

        public static Elf Create<TBattleScript>(ElfID ID,string name,string texture,Color color,int baseHeight,FrameSet[] frameSets) where TBattleScript:BattleScript,new() {
            return new(ID,name,()=>new TBattleScript(),texture,color,baseHeight,frameSets);
        }

        public static Elf Create(ElfID ID,string name,Func<BattleScript> scriptGenerator,string texture,Color color,int baseHeight,FrameSet[] frameSets) {
            return new(ID,name,scriptGenerator,texture,color,baseHeight,frameSets);
        }
    }
}
