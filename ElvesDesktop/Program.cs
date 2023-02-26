using System;
using System.IO;
using TwelveEngine.Shell;
using TwelveEngine;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ElvesDesktop {
    public sealed class Program:EntryPoint {

        [STAThread]
        public static void Main(string[] args) {
            var program = new Program();
            program.StartEngine(args);
        }

        private void StartEngine(string[] args) {
            string saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),Elves.Constants.SaveFolder);
            EngineMain(saveDirectory,args);
        }

        protected override void OnGameLoad(GameStateManager game,string saveDirectory) {
            Elves.Program.Start(game,saveDirectory,OpenURL);
        }

        protected override void OnGameCrashed() {
            Elves.Program.OnGameCrashed();
        }

        private static bool OpenURL(string url) {
            /* https://stackoverflow.com/a/43232486 */
            try {
                Process.Start(url);
                return true;
            } catch {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    url = url.Replace("&","^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                } else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    Process.Start("xdg-open",url);
                } else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                    Process.Start("open",url);
                }
                return false;
            }
        }
    }
}
