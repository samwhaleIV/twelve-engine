using System;

namespace TwelveEngine.Shell.UI {
    public static class TimeWriters {
        private static readonly Action<GameManager,DebugWriter>[] writers = new Action<GameManager,DebugWriter>[] {

            static (game,writer) => {
                writer.Write(game.State?.Now ?? TimeSpan.Zero,"Local Time");
            },

            static (game,writer) => {
                writer.Write(game.State?.RealTime ?? TimeSpan.Zero,"Local Realtime");
            },

            static (game,writer) => {
                writer.Write(game.ProxyTime.Now,"Global Time");
            },

            static (game,writer) => {
                writer.Write(game.ProxyTime.RealTime,"Global Realtime");
            },

            static (game,writer) => {
                writer.Write(game.ProxyTime.PauseTime,"Pause");
                writer.Write(game.ProxyTime.Drift,"Drift");
                writer.Write(ProxyGameTime.GetElapsedTime(),"Lifetime");
            }
        };

        public static int Count => writers.Length;
        public static Action<GameManager,DebugWriter> Get(int index) => writers[index];
    }
}
