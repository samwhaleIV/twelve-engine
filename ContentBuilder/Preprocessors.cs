using System;
using System.Collections.Generic;
using System.Text;

namespace ContentBuilder {
    internal sealed class Preprocessors:Dictionary<string,Action<StringBuilder>> {

        public string ChromaKey { get; set; }
        public string TextureFormat { get; set; }

        public Preprocessors() {
            Add(".jpg", AddImage);
            Add(".jpeg",AddImage);
            Add(".png", AddImage);
            Add(".spritefont", AddSpriteFont);
            Add(".dae", AddModel);
        }

        private void AddImage(StringBuilder builder) {
            builder.AppendLine("/importer:TextureImporter");
            builder.AppendLine("/processor:TextureProcessor");
            builder.AppendLine($"/processorParam:ColorKeyColor={ ChromaKey }");
            builder.AppendLine("/processorParam:ColorKeyEnabled=True");
            builder.AppendLine("/processorParam:GenerateMipmaps=False");
            builder.AppendLine("/processorParam:PremultiplyAlpha=True");
            builder.AppendLine("/processorParam:ResizeToPowerOfTwo=False");
            builder.AppendLine("/processorParam:MakeSquare=False");
            builder.AppendLine($"/processorParam:TextureFormat={TextureFormat}");
        }

        private void AddSpriteFont(StringBuilder builder) {
            builder.AppendLine("/importer:FontDescriptionImporter");
            builder.AppendLine("/processor:FontDescriptionProcessor");
            builder.AppendLine("/processorParam:PremultiplyAlpha=True");
            builder.AppendLine("/processorParam:TextureFormat=Compressed");
        }

        private void AddModel(StringBuilder builder) {
            builder.AppendLine("/importer:FbxImporter"); /* Why MONOGAME? FOR THE LOVE OF GOD WHY. IT'S NOT EVEN AN FBX FILE */
            builder.AppendLine("/processor:ModelProcessor");
            builder.AppendLine("/processorParam:ColorKeyColor=0,0,0,0");
            builder.AppendLine("/processorParam:ColorKeyEnabled=True");
            builder.AppendLine("/processorParam:DefaultEffect=BasicEffect");
            builder.AppendLine("/processorParam:GenerateMipmaps=True");
            builder.AppendLine("/processorParam:GenerateTangentFrames=False");
            builder.AppendLine("/processorParam:PremultiplyTextureAlpha=True");
            builder.AppendLine("/processorParam:PremultiplyVertexColors=True");
            builder.AppendLine("/processorParam:ResizeTexturesToPowerOfTwo=False");
            builder.AppendLine("/processorParam:RotationX=0");
            builder.AppendLine("/processorParam:RotationY=0");
            builder.AppendLine("/processorParam:RotationZ=0");
            builder.AppendLine("/processorParam:Scale=1");
            builder.AppendLine("/processorParam:SwapWindingOrder=False");
            builder.AppendLine("/processorParam:TextureFormat=Color");
        }

    }
}
