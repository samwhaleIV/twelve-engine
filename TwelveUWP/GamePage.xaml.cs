using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TwelveEngine;
using MonoGame.Framework;

namespace TwelveUWP {
    public sealed partial class GamePage:Page {
        private readonly GameManager game;
        public GamePage() {
            this.InitializeComponent();

            var launchArguments = string.Empty;

            var window = Window.Current.CoreWindow;
            game = XamlGame<GameManager>.Create(launchArguments,window,swapChainPanel);

            TwelveEngine.Program.ConfigStartGame(game);
        }
    }
}
