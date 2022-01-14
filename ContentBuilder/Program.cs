using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace ContentBuilder {
    internal static class Program {

        private const string ENGINE_FOLDER = "TwelveEngine";
        private const string CONTENT_FOLDER = "Content";

        private const string PLATFORM = "DesktopGL";
        private const string GRAPHICS_PROFILE = "Reach";

        private const string CHROMA_KEY = "255,0,255,255";

        private static readonly Dictionary<string,Action<StringBuilder,string>> FileProcessors = new Dictionary<string,Action<StringBuilder,string>>() {
            { ".jpg", ImageProcessor },
            { ".jpeg", ImageProcessor },
            { ".png", ImageProcessor },
            { ".spritefont", SpriteFontProcessor }
        };

        private static string GetRelativePathName(string file) {
            var fileSplit = file.Split(Path.DirectorySeparatorChar);

            var contentStart = Array.IndexOf(fileSplit,CONTENT_FOLDER);
            var path = string.Join('/',fileSplit,contentStart,fileSplit.Length-contentStart);

            return path;
        }

        private static void ImageProcessor(StringBuilder builder,string path) {
            builder.AppendLine("/importer:TextureImporter");
            builder.AppendLine("/processor:TextureProcessor");
            builder.AppendLine($"/processorParam:ColorKeyColor={CHROMA_KEY}");
            builder.AppendLine("/processorParam:ColorKeyEnabled=True");
            builder.AppendLine("/processorParam:GenerateMipmaps=False");
            builder.AppendLine("/processorParam:PremultiplyAlpha=True");
            builder.AppendLine("/processorParam:ResizeToPowerOfTwo=False");
            builder.AppendLine("/processorParam:MakeSquare=False");
            builder.AppendLine("/processorParam:TextureFormat=Color");
            builder.AppendLine($"/build:../{path};Content/{Path.GetFileName(path)}");
            builder.AppendLine();
        }

        private static void SpriteFontProcessor(StringBuilder builder,string path) {
            builder.AppendLine("/importer:FontDescriptionImporter");
            builder.AppendLine("/processor:FontDescriptionProcessor");

            builder.AppendLine("/processorParam:PremultiplyAlpha=True");
            builder.AppendLine("/processorParam:TextureFormat=Compressed");
            builder.AppendLine($"/build:../{path};Content/{Path.GetFileName(path)}");
            builder.AppendLine();
        }

        private static void AddFile(StringBuilder builder,string file) {
            var extension = Path.GetExtension(file);

            if(!FileProcessors.ContainsKey(extension)) {
                Console.WriteLine($"No file processor for type '{extension}'");
                return;
            } else {
                FileProcessors[extension].Invoke(builder,GetRelativePathName(file));
                Console.WriteLine($"'{Path.GetFileName(file)}' added to manifest");
            }
        }

        private static void SymbolizeFolder(StringBuilder builder,string folder) {

            var directories = Directory.GetDirectories(folder,"*",SearchOption.AllDirectories);
            var files = Directory.GetFiles(folder,"*",SearchOption.TopDirectoryOnly).ToList();

            foreach(var directory in directories) {
                files.AddRange(Directory.GetFiles(directory,"*",SearchOption.TopDirectoryOnly));
            }

            foreach(var file in files) {
                AddFile(builder,file);
            }
        }

        private static string GetTopLevelName(string folder) {
            var path = folder.Split(Path.DirectorySeparatorChar);
            return path[path.Length - 1];
        }

        private static void AddGeneratorSettings(StringBuilder builder) {
            builder.AppendLine($"/platform:{PLATFORM}");
            builder.AppendLine("/outputDir:Output/obj/$(Platform)");
            builder.AppendLine("/intermediateDir:Output/bin/$(Platform)");
            builder.AppendLine("/config:");
            builder.AppendLine($"/profile:{GRAPHICS_PROFILE}");
            builder.AppendLine("/compress:False");
            //builder.AppendLine("/incremental");
            builder.AppendLine();
        }

        private static void CreateMGCBFile(string path,string defaultContent,string folder) {
            var builder = new StringBuilder();

            builder.AppendLine("# ---- Start Auto-Generated MGCB File ---- #");
            builder.AppendLine();

            AddGeneratorSettings(builder);

            builder.Append(defaultContent);

            AddFolder(builder,folder);

            builder.AppendLine("# ---- End Auto-Generated MGCB File ---- #");

            File.WriteAllText(path,builder.ToString());
        }

        private static void AddFolder(StringBuilder builder,string folder) {
            var topLevelName = GetTopLevelName(folder);

            builder.Append("# ---- Start ");
            builder.Append(topLevelName);
            builder.AppendLine(" ---- #");
            builder.AppendLine();

            SymbolizeFolder(builder,folder);

            builder.Append("# ---- End ");
            builder.Append(topLevelName);
            builder.AppendLine(" ---- #");
            builder.AppendLine();
        }

        private static string CreateDefaultContent(string folder) {
            var builder = new StringBuilder();
            AddFolder(builder,folder);
            return builder.ToString();
        }

        internal static void Main() {

            var rootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),@"..\..\..\..\"));
            var contentFolder = Path.Combine(rootPath,CONTENT_FOLDER);

            var engineContentFolder = Path.Combine(contentFolder,ENGINE_FOLDER);

            var defaultContent = CreateDefaultContent(engineContentFolder);

            var contentDirectories = Directory.GetDirectories(contentFolder).ToList();
            contentDirectories.Remove(engineContentFolder);

            var badFolders = new List<string>() { "bin", "obj", "Output" };

            foreach(var folder in contentDirectories) {
                var name = GetTopLevelName(folder);
                if(badFolders.Contains(name)) {
                    continue;
                }
                var outputFile = Path.Combine(contentFolder,name) + ".mgcb";
                CreateMGCBFile(outputFile,defaultContent,folder);
            }

        }
    }
}
