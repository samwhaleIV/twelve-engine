using System;

namespace TwelveEngine.Shell.UI {
    public static class TimeWriters {
        private static readonly Action<GameManager,DebugWriter>[] writers = new Action<GameManager,DebugWriter>[] {

            static (game,writer) => {
                writer.Write("[Game Time]");
                writer.Write(game.State?.Now ?? TimeSpan.Zero,"T");
                writer.Write(game.State?.RealTime ?? TimeSpan.Zero,"RT");
            },

            static (game,writer) => {
                writer.Write("[Local Time]");
                writer.Write(game.State?.LocalNow ?? TimeSpan.Zero,"T");
                writer.Write(game.State?.LocalRealTime ?? TimeSpan.Zero,"RT");
            },

            static (game,writer) => {
                writer.Write("[Global Time]");
                writer.Write(game.ProxyTime.PauseTime,"P");
                writer.Write(game.ProxyTime.Drift,"D");
                writer.Write(ProxyGameTime.GetElapsedTime(),"LT");

            }
        };

        public static int Count => writers.Length;
        public static Action<GameManager,DebugWriter> Get(int index) => writers[index];
    }
}
