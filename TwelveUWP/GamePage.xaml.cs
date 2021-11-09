using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TwelveEngine;
using MonoGame.Framework;

namespace TwelveUWP {
    public sealed partial class GamePage:Page {
        internal readonly GameManager _game;
        public GamePage() {
            this.InitializeComponent();

            var launchArguments = string.Empty;

            var window = Window.Current.CoreWindow;
            _game = XamlGame<GameManager>.Create(launchArguments,window,swapChainPanel);
        }
    }
}
