namespace TwelveEngine.Shell.UI {
    internal static class TimeWriters {
        private static readonly Action<GameStateManager,DebugWriter>[] writers = new Action<GameStateManager,DebugWriter>[] {

            static (game,writer) => {
                writer.Write("[Game Time]");
                writer.Write(ProxyTime.Now,"T");
                writer.Write(ProxyTime.RealTime,"RT");
            },

            static (game,writer) => {
                writer.Write("[Local Time]");
                writer.Write(game.GameState?.LocalNow ?? TimeSpan.Zero,"T");
                writer.Write(game.GameState?.LocalRealTime ?? TimeSpan.Zero,"RT");
            },

            static (game,writer) => {
                writer.Write("[Global Time]");
                writer.Write(ProxyTime.PauseTime,"P");
                writer.Write(ProxyTime.Drift,"D");
                writer.Write(ProxyTime.GetElapsedTime(),"LT");
            }
        };

        internal static int Count => writers.Length;
        internal static Action<GameStateManager,DebugWriter> Get(int index) => writers[index];
    }
}
