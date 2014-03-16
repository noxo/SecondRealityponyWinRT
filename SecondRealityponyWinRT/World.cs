using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace SecondRealipony
{
    class World : SRSegment
    {
        public override float EndBeat { get { return 106; } }
        public override string MusicName { get { return "World"; } }
        public override float BeatLength { get { return 60F / 125F; } }

        Windows.UI.Xaml.Controls.MediaElement videoPlayer;
        bool playing;

        public World(Game game)
            : base(game)
        {
        }

        protected override void DrawSegment()
        {
            // OMG this thingy were prerendered video :)

            if (!playing)
            {
                SecondRealityponyWinRT.GamePage gamePage = Windows.UI.Xaml.Window.Current.Content as SecondRealityponyWinRT.GamePage;
                videoPlayer = gamePage.TheVideoPlayer;
                videoPlayer.Source = new Uri("ms-appx:/Content/World.wmv", UriKind.Absolute);
                videoPlayer.Play();
                videoPlayer.MediaEnded += videoPlayer_MediaEnded;
                playing = true;
            }
         
        }

        void videoPlayer_MediaEnded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            videoPlayer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}
