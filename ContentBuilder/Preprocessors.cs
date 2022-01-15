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
            builder.AppendLine($"/processorParam:TextureFormat={ TextureFormat }");
        }

        private void AddSpriteFont(StringBuilder builder) {
            builder.AppendLine("/importer:FontDescriptionImporter");
            builder.AppendLine("/processor:FontDescriptionProcessor");
            builder.AppendLine("/processorParam:PremultiplyAlpha=True");
            builder.AppendLine("/processorParam:TextureFormat=Compressed");
        }

    }
}
