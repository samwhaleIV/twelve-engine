using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TwelveEngine {
    public readonly struct CPUTexture {

        internal CPUTexture(int width,int height,Color[] data,string name) {
            Width = width;
            Height = height;
            Data = data;
            Name = name;
        }

        public readonly int Width;
        public readonly int Height;
        public readonly Color[] Data;
        public readonly string Name;

        public static readonly CPUTexture Default = new CPUTexture(0,0,new Color[0],null);

        private static Dictionary<string,CPUTexture> cpuTextures;

        public static CPUTexture Get(string name) {
            if(!cpuTextures.ContainsKey(name)) {
                return Default;
            }
            return cpuTextures[name];
        }

        internal static void LoadDictionary(ContentManager content) {
            if(cpuTextures != null) {
                return;
            }
            cpuTextures = new Dictionary<string,CPUTexture>();

            foreach(var name in Constants.CPUTextures) {
                var texture = content.Load<Texture2D>(name);

                int width = texture.Width;
                int height = texture.Height;

                Color[] buffer = new Color[width * height];
                texture.GetData(buffer);

                var offThreadTexture = new CPUTexture(width,height,buffer,name);
                cpuTextures[name] = offThreadTexture;
            }
        }
    }
}
