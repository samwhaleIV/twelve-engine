using System.IO;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;

namespace TwelveEngine {
    public static class JSEngine {

        private static readonly V8ScriptEngine engine = new V8ScriptEngine();
        public static void RunTest() {
            engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;

            var searchPath = Path.Combine(Directory.GetCurrentDirectory(),"Scripts");
            engine.DocumentSettings.SearchPath = searchPath;

            var script = engine.CompileDocument("test-script.js",ModuleCategory.Standard);

            var result = engine.Evaluate(script);
            Runtime.Log($"JSEngine Test Result: {result}");
        }
    }
}
