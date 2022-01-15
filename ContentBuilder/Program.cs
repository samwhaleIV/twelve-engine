using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ContentBuilder {
    internal static class Program {

        private const string ENGINE_FOLDER = "TwelveEngine";
        private const string CONTENT_FOLDER = "Content";
        private const string CHROMA_KEY = "255,0,255,255";
        private const string MGCB_EXTENSION = ".mgcb";

        /* Is Path.Combine'ed to Directory.GetCurrentDirectory() */
        private const string ENGINE_ROOT_RESOLVER = @"..\..\..\..\"; 

        private static readonly string Platform, GraphicsProfile;

        static Program() {
            Platform = Environment.GetEnvironmentVariable("platform");
            GraphicsProfile = Environment.GetEnvironmentVariable("profile");
        }

        private static readonly Dictionary<string,Action<StringBuilder,string>> Preprocessors = new Dictionary<string,Action<StringBuilder,string>>() {
            { ".jpg", AddImage },
            { ".jpeg", AddImage },
            { ".png", AddImage },
            { ".spritefont", AddSpriteFont }
        };

        private static string GetFileName(string path) {
            string[] segments = path.Split(Path.DirectorySeparatorChar);
            int contentStart = Array.IndexOf(segments,CONTENT_FOLDER);
            string fileName = string.Join('/',segments,contentStart,segments.Length-contentStart);
            return fileName;
        }

        private static void AddImage(StringBuilder builder,string path) {
            builder.AppendLine("/importer:TextureImporter");
            builder.AppendLine("/processor:TextureProcessor");
            builder.AppendLine($"/processorParam:ColorKeyColor={CHROMA_KEY}");
            builder.AppendLine("/processorParam:ColorKeyEnabled=True");
            builder.AppendLine("/processorParam:GenerateMipmaps=False");
            builder.AppendLine("/processorParam:PremultiplyAlpha=True");
            builder.AppendLine("/processorParam:ResizeToPowerOfTwo=False");
            builder.AppendLine("/processorParam:MakeSquare=False");
            builder.AppendLine("/processorParam:TextureFormat=Color");
            builder.AppendLine($"/build:../{path};{Path.GetFileName(path)}");
            builder.AppendLine();
        }

        private static void AddSpriteFont(StringBuilder builder,string path) {
            builder.AppendLine("/importer:FontDescriptionImporter");
            builder.AppendLine("/processor:FontDescriptionProcessor");

            builder.AppendLine("/processorParam:PremultiplyAlpha=True");
            builder.AppendLine("/processorParam:TextureFormat=Compressed");
            builder.AppendLine($"/build:../{path};{Path.GetFileName(path)}");
            builder.AppendLine();
        }

        private static void AddFile(StringBuilder builder,string path) {
            string extension = Path.GetExtension(path);
            string name = GetFileName(path);
            if(!Preprocessors.ContainsKey(extension)) {
                Console.WriteLine($"No file processor for '{name}' of type '{extension}'");
                return;
            } else {
                Preprocessors[extension].Invoke(builder,name);
                Console.WriteLine($"'{name}' added to manifest");
            }
        }

        private static string GetContentDirectoryName(string directory) {
            string[] segments = directory.Split(Path.DirectorySeparatorChar);
            return segments[segments.Length - 1];
        }

        private static void AddMGCBSettings(StringBuilder builder) {
            builder.AppendLine($"/platform:{Platform}");
            builder.AppendLine($"/profile:{GraphicsProfile}");
            builder.AppendLine("/compress:False");
            builder.AppendLine();
        }

        private static string GetMGCBFile(string directory,string defaultContent) {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# DO NOT EXECUTE OUTSIDE OF A BUILD PROCESS #");
            builder.AppendLine();
            builder.AppendLine("# ---- Start Auto-Generated MGCB File ---- #");
            builder.AppendLine();

            AddMGCBSettings(builder);
            builder.Append(defaultContent);
            AddDirectory(builder,directory);

            builder.AppendLine("# ---- End Auto-Generated MGCB File ---- #");
            
            return builder.ToString();
        }

        private static void AddDirectory(StringBuilder builder,string directory) {
            string name = GetContentDirectoryName(directory);

            builder.Append("# ---- Start ");
            builder.Append(name);
            builder.AppendLine(" ---- #");
            builder.AppendLine();

            string[] files = Directory.GetFiles(directory,"*",SearchOption.AllDirectories);
            foreach(string file in files) AddFile(builder,file);

            builder.Append("# ---- End ");
            builder.Append(name);
            builder.AppendLine(" ---- #");
            builder.AppendLine();
        }

        private static string GetDirectoryContent(string directory) {
            StringBuilder builder = new StringBuilder();
            AddDirectory(builder,directory);
            return builder.ToString();
        }

        private static string GetContentRoot() {
            string twelveEnginePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),ENGINE_ROOT_RESOLVER));
            return Path.Combine(twelveEnginePath,CONTENT_FOLDER);
        }

        private static string GetEngineContent(string contentRoot) {
            string directory = Path.Combine(contentRoot,ENGINE_FOLDER);
            return GetDirectoryContent(directory);
        }

        internal static void Main() {
            string contentRoot = GetContentRoot();
            string[] contentDirectories = Directory.GetDirectories(contentRoot);

            HashSet<string> badDirectories = new HashSet<string>() { "bin", "obj", ENGINE_FOLDER };

            string defaultContent = GetEngineContent(contentRoot);

            foreach(string directory in contentDirectories) {
                string name = GetContentDirectoryName(directory);
                if(badDirectories.Contains(name)) continue;

                string outputFile = Path.Combine(contentRoot,name) + MGCB_EXTENSION;
                string fileContents = GetMGCBFile(directory,defaultContent);

                File.WriteAllText(outputFile,fileContents);
            }
        }
    }
}
