using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TwelveEngine;
using MonoGame.Framework;

namespace TwelveUWP {
    public sealed partial class GamePage:Page {
        internal readonly TestGame _game;
        public GamePage() {
            this.InitializeComponent();

            var launchArguments = string.Empty;

            var window = Window.Current.CoreWindow;
            _game = XamlGame<RuntimeStartTarget>.Create(launchArguments,window,swapChainPanel);
        }
    }
}
