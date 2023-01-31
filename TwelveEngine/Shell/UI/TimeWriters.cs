using System;

namespace TwelveEngine.Shell.UI {
    public static class TimeWriters {
        private static readonly Action<GameManager,DebugWriter>[] writers = new Action<GameManager,DebugWriter>[] {

            static (game,writer) => {
                writer.Write("[Game Time]");
                writer.Write(ProxyTime.Now,"T");
                writer.Write(ProxyTime.RealTime,"RT");
            },

            static (game,writer) => {
                writer.Write("[Local Time]");
                writer.Write(game.State?.LocalNow ?? TimeSpan.Zero,"T");
                writer.Write(game.State?.LocalRealTime ?? TimeSpan.Zero,"RT");
            },

            static (game,writer) => {
                writer.Write("[Global Time]");
                writer.Write(ProxyTime.PauseTime,"P");
                writer.Write(ProxyTime.Drift,"D");
                writer.Write(ProxyTime.GetElapsedTime(),"LT");
            }
        };

        public static int Count => writers.Length;
        public static Action<GameManager,DebugWriter> Get(int index) => writers[index];
    }
}
