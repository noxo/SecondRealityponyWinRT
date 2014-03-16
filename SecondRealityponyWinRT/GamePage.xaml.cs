using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MonoGame.Framework;

using SecondRealipony;

namespace SecondRealityponyWinRT
{
    /// <summary>
    /// The root page used to display the game.
    /// </summary>
    public sealed partial class GamePage : SwapChainBackgroundPanel
    {
        readonly SRController _game;

        public GamePage(string launchArguments)
        {
            this.InitializeComponent();

            // Create the game.
            _game = XamlGame<SRController>.Create(launchArguments, Window.Current.CoreWindow, this);
        }

        public MediaElement TheVideoPlayer
        {
            get
            {
                return VideoPlayer;
            }
        }
    }
}
