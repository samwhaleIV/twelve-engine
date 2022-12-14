using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TwelveEngine {
    public readonly struct CPUTexture {

        private static Color[,] convertFlatData(Color[] data,int width,int height) {
            var data2D = new Color[width,height];
            for(int i = 0;i<data.Length;i++) {
                int x = i % width;
                int y = i / width;
                data2D[x,y] = data[i];
            }
            return data2D;
        }

        internal CPUTexture(Color[] data,int width,int height,string name) {
            Data = convertFlatData(data,width,height);
            Width = width;
            Height = height;
            Name = name;
        }

        public readonly int Width;
        public readonly int Height;
        public readonly Color[,] Data;
        public readonly string Name;

        public static readonly CPUTexture Default = new CPUTexture(new Color[0],0,0,null);

        private static Dictionary<string,CPUTexture> cpuTextures;

        public static CPUTexture Get(string name) {
            if(!cpuTextures.ContainsKey(name)) {
                return Default;
            }
            return cpuTextures[name];
        }

        internal static void LoadDictionary(ContentManager content,string[] textureNames) {
            if(cpuTextures != null) {
                return;
            }
            cpuTextures = new Dictionary<string,CPUTexture>();

            foreach(var name in textureNames) {
                var texture = content.Load<Texture2D>(name);

                int width = texture.Width, height = texture.Height;

                Color[] buffer = new Color[width * height];
                texture.GetData(buffer);

                var offThreadTexture = new CPUTexture(buffer,width,height,name);
                cpuTextures[name] = offThreadTexture;
            }
        }
    }
}
