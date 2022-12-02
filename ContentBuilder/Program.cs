using System.Text;

namespace ContentBuilder {
    internal static class Program {

        private const string ENGINE_FOLDER = "TwelveEngine";
        private const string CONTENT_FOLDER = "Content";
        private const string MGCB_EXTENSION = ".mgcb";

        private const string CHROMA_KEY = "255,0,255,255";
        private const string TEXTURE_FORMAT = "Color";

        private const bool COMPRESS_CONTENT = false;

        private const string ASSEMBLY_REFERENCE_FOLDER = "BuilderAssemblies";

        private const string DEFAULT_PLATFORM = "DesktopGL";
        private const string DEFAULT_GRAPHICS_PROFILE = "HiDef";

        /* Is Path.Combine'ed to Directory.GetCurrentDirectory() */
        private const string ENGINE_ROOT_RESOLVER = @"..\..\..\..\";
        private const bool PREFIX_ENGINE_NAMESPACE = false;

        private const string BIN_FOLDER = "bin", OBJ_FOLDER = "obj";

        private static readonly string Platform, GraphicsProfile;

        static Program() {
            Platform = Environment.GetEnvironmentVariable("platform") ?? DEFAULT_PLATFORM;
            GraphicsProfile = Environment.GetEnvironmentVariable("profile") ?? DEFAULT_GRAPHICS_PROFILE;
        }

        private static readonly Preprocessors preprocessors = new Preprocessors() {
            ChromaKey = CHROMA_KEY,
            TextureFormat = TEXTURE_FORMAT
        };

        private static string GetOutputPath(string path,string outputRoot) {
            string[] segments = path.Split('/');
            path = string.Join('/',segments,1,segments.Length-1);
            return string.IsNullOrEmpty(outputRoot) ? path : $"{outputRoot}/{path}";
        }

        private static void AddBuildPath(StringBuilder builder,string path,string outputRoot) {
            string outputPath = GetOutputPath(path,outputRoot);
            builder.AppendLine($"/build:{path};{outputPath}");
        }

        private static string GetShortPath(string path) {
            string[] segments = path.Split(Path.DirectorySeparatorChar);
            int contentStart = Array.IndexOf(segments,CONTENT_FOLDER);
            return string.Join('/',segments,contentStart+1,segments.Length-contentStart-1);
        }

        private static void AddFile(StringBuilder builder,string path,string outputRoot) {
            string extension = Path.GetExtension(path);
            string shortPath = GetShortPath(path);
            if(!preprocessors.ContainsKey(extension)) {
                Console.WriteLine($"No file processor for '{shortPath}' of type '{extension}'");
                return;
            } else {
                Console.WriteLine($"Add '{path}'");
                preprocessors[extension].Invoke(builder);
                AddBuildPath(builder,shortPath,outputRoot);
                builder.AppendLine();
            }
        }

        private static string GetContentDirectoryName(string directory) {
            string[] segments = directory.Split(Path.DirectorySeparatorChar);
            return segments[segments.Length - 1];
        }

        private static void AddMGCBSettings(StringBuilder builder) {
            builder.AppendLine($"/outputDir:{BIN_FOLDER}");
            builder.AppendLine($"/intermediateDir:{OBJ_FOLDER}");
            builder.AppendLine($"/platform:{Platform}");
            builder.AppendLine("/config:");
            builder.AppendLine($"/profile:{GraphicsProfile}");

            string compress = COMPRESS_CONTENT ? "True" : "False";
            builder.AppendLine($"/compress:{compress}");

            builder.AppendLine("/clean");
            builder.AppendLine();
        }

        private static void AddBuilderAssemblies(StringBuilder builder) {
            string assemblyDirectory = Path.Combine(GetContentRoot(),ASSEMBLY_REFERENCE_FOLDER);
            if(!File.Exists(assemblyDirectory)) {
                return;
            }
            string[] assemblies = Directory.GetFiles(assemblyDirectory,"*.dll");
            foreach(string assembly in assemblies) {
                var fileName = Path.GetFileName(assembly);
                builder.Append($"/reference:{ASSEMBLY_REFERENCE_FOLDER}/");
                builder.AppendLine(fileName);
            }
            builder.AppendLine();
        }

        private static string GetMGCBFile(string directory,string defaultContent,string outputRoot) {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# DO NOT EXECUTE OUTSIDE OF A BUILD PROCESS #");
            builder.AppendLine();
            builder.AppendLine("# ---- Start Auto-Generated MGCB File ---- #");
            builder.AppendLine();

            AddMGCBSettings(builder);
            AddBuilderAssemblies(builder);

            builder.Append(defaultContent);
            AddDirectory(builder,directory,outputRoot);

            builder.AppendLine("# ---- End Auto-Generated MGCB File ---- #");
            
            return builder.ToString();
        }

        private static void AddDirectory(StringBuilder builder,string directory,string outputRoot) {
            string name = GetContentDirectoryName(directory);

            builder.Append("# ---- Start ");
            builder.Append(name);
            builder.AppendLine(" ---- #");
            builder.AppendLine();

            string[] files = Directory.GetFiles(directory,"*",SearchOption.AllDirectories);
            foreach(string file in files) AddFile(builder,file,outputRoot);

            builder.Append("# ---- End ");
            builder.Append(name);
            builder.AppendLine(" ---- #");
            builder.AppendLine();
        }

        private static string GetDirectoryContent(string directory,string outputRoot) {
            StringBuilder builder = new StringBuilder();
            AddDirectory(builder,directory,outputRoot);
            return builder.ToString();
        }

        private static string GetContentRoot() {
            string twelveEnginePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),ENGINE_ROOT_RESOLVER));
            return Path.Combine(twelveEnginePath,CONTENT_FOLDER);
        }

        private static string GetEngineContent(string contentRoot,string outputRoot) {
            string directory = Path.Combine(contentRoot,ENGINE_FOLDER);
            return GetDirectoryContent(directory,outputRoot);
        }

        private static void DeleteMGCBCache() {
            /* Forces the content pipeline to rebuild this MGCB file */
            string bin = BIN_FOLDER;
            if(Directory.Exists(bin)) {
                Directory.Delete(bin);
            }
            string obj = OBJ_FOLDER;
            if(Directory.Exists(obj)) {
                Directory.Delete(obj);
            }
        }

        private static void AssertContentRoot(string contentRoot) {
            if(!Directory.Exists(contentRoot)) {
                throw new Exception("Missing content root folder!");
            }
            string engineFolder = Path.Combine(contentRoot,ENGINE_FOLDER);
            if(!Directory.Exists(engineFolder)) {
                throw new Exception($"Missing default engine content folder! ({engineFolder})");
            }
        }

        private static HashSet<string> GetIgnoredDirectories() {
            return new HashSet<string>() { BIN_FOLDER, OBJ_FOLDER, ENGINE_FOLDER, ASSEMBLY_REFERENCE_FOLDER };
        }

        internal static void Main() {
            string contentRoot = GetContentRoot();
            AssertContentRoot(contentRoot);

            string[] contentDirectories = Directory.GetDirectories(contentRoot);

            HashSet<string> ignoredDirectories = GetIgnoredDirectories();

            string defaultContent = GetEngineContent(contentRoot,PREFIX_ENGINE_NAMESPACE ? ENGINE_FOLDER : string.Empty);

            foreach(string directory in contentDirectories) {
                string directoryName = GetContentDirectoryName(directory);
                if(ignoredDirectories.Contains(directoryName)) continue;

                string outputFile = Path.Combine(contentRoot,directoryName) + MGCB_EXTENSION;
                string fileContents = GetMGCBFile(directory,defaultContent,string.Empty);

                File.WriteAllText(outputFile,fileContents);
                Console.WriteLine($"Exported MGCB file: '{outputFile}'");
            }
        }
    }
}
