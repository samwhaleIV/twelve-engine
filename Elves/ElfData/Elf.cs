using Elves.Animation;
using Elves.Battle;
using Elves.Battle.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Elves.ElfData {
    public readonly struct Elf {

        public readonly Texture2D Texture;
        public readonly ElfID ID;
        public readonly string Name;
        public readonly int BaseHeight;
        public readonly Dictionary<AnimationType,FrameSet> FrameSets;
        public readonly Color Color;
        public readonly Func<BattleScript> ScriptGenerator;
        public readonly Rectangle PreviewSource;

        private static Rectangle GetPreviewSource(FrameSet[] frameSets,Point textureSize) {
            foreach(var frame in frameSets) {
                AnimationType type = frame.AnimationType;
                if(frame.FrameCount < 1 || type != AnimationType.Idle && type != AnimationType.Static) {
                    continue;
                }
                return frame.Frames[0];
            }
            return new(Point.Zero,textureSize);
        }

        private static Dictionary<AnimationType,FrameSet> CreateFrameSetDictionary(FrameSet[] frameSets) {
            Dictionary<AnimationType,FrameSet> dictionary = new();
            foreach(FrameSet frameSet in frameSets) {
                dictionary[frameSet.AnimationType] = frameSet;
            }
            return dictionary;
        }

        private Elf(ElfID ID,string name,Func<BattleScript> scriptGenerator,string texture,Color color,int baseHeight,FrameSet[] frameSets) {
            ScriptGenerator = scriptGenerator;
            Texture = Program.Game.Content.Load<Texture2D>(texture);
            this.ID = ID;
            Name = name;
            BaseHeight = baseHeight;
            FrameSets = CreateFrameSetDictionary(frameSets);
            Color = color;
            PreviewSource = GetPreviewSource(frameSets,Texture.Bounds.Size);
        }

        public static Elf Create<TBattleScript>(ElfID ID,string name,string texture,Color color,int baseHeight,FrameSet[] frameSets) where TBattleScript:BattleScript,new() {
            return new(ID,name,()=>new TBattleScript(),texture,color,baseHeight,frameSets);
        }

        public static Elf Create(ElfID ID,string name,Func<BattleScript> scriptGenerator,string texture,Color color,int baseHeight,FrameSet[] frameSets) {
            return new(ID,name,scriptGenerator,texture,color,baseHeight,frameSets);
        }
    }
}
