using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TwelveEngine {
    public readonly struct OffThreadTexture {

        internal OffThreadTexture(int width,int height,Color[] data,string name) {
            Width = width;
            Height = height;
            Data = data;
            Name = name;
        }

        public readonly int Width;
        public readonly int Height;
        public readonly Color[] Data;
        public readonly string Name;

        public static readonly OffThreadTexture Default = new OffThreadTexture(0,0,new Color[0],null);

        private static Dictionary<string,OffThreadTexture> offThreadTextures;

        public static OffThreadTexture Get(string name) {
            if(!offThreadTextures.ContainsKey(name)) {
                return Default;
            }
            return offThreadTextures[name];
        }

        internal static void LoadDictionary(ContentManager content) {
            if(offThreadTextures != null) {
                return;
            }
            offThreadTextures = new Dictionary<string,OffThreadTexture>();

            foreach(var name in Constants.OffThreadTextures) {
                var texture = content.Load<Texture2D>(name);

                int width = texture.Width;
                int height = texture.Height;

                Color[] buffer = new Color[width * height];
                texture.GetData(buffer);

                var offThreadTexture = new OffThreadTexture(width,height,buffer,name);
                offThreadTextures[name] = offThreadTexture;
            }
        }
    }
}
