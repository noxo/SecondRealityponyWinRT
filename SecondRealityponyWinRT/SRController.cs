using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace SecondRealipony
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SRController : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        bool VideoMode = false;
        double VideoFrameRate = 60D; //60000D/1001D;

        int FrameNumber = 0;
        int FrameOffset = 0;
        int SegmentNumber = 0;
        Type[] segmentTypes;
        SRSegment[] segments;
        SRSegment currentSegment { get { return SegmentNumber < segments.Length ? segments[SegmentNumber] : null; } }
        TimeSpan SegmentStartTime;

        public SRController()
        {

            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            graphics.PreferMultiSampling = true;
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            graphics.ApplyChanges();

            
            Content.RootDirectory = "Content";
            IsFixedTimeStep = !VideoMode;

            segmentTypes = GetStandardSegmentOrder();
        }

        public Type[] GetStandardSegmentOrder()
        {
            return new Type[] {
                typeof(Intro),
                typeof(Title),
                typeof(Twilight),
                typeof(Rarity),
#if !SURFACE_RT 
                typeof(Vinyl), // too big textures generated for surface rt 1gen
#endif
                typeof(GetDown),
                typeof(Rainbow),
                typeof(EndFirstHalf),
                typeof(Fluttershy),
#if !SURFACE_RT
                typeof(Applejack), // HRESULT: 0x887A0005 with surface rt 1gen
#endif
                typeof(Cmc),
                typeof(Cube),
                typeof(Pinkie),
                typeof(Derpy),
//                typeof(Waves), <--- looks like a bug in SharpDX/Monogame, crashy crashy with this scene
                typeof(EndSecondHalf),
                typeof(WorldStart),
                typeof(World),
                typeof(Thanks),
                typeof(Credits),
                typeof(End)
            };
        }



        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //This instantiates all the segments, and they all load their content in their constructors
            segments = segmentTypes.Select(T => (SRSegment)Activator.CreateInstance(T, this)).ToArray();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            TimeSpan span = (VideoMode ? GetTimespan(FrameNumber, VideoFrameRate) : gameTime.TotalGameTime) - SegmentStartTime;

            //System.Console.WriteLine("Draw called: " + gameTime.TotalGameTime.ToString() + " " + span.ToString());

            //Rules for timing: Each segment reports that it has completed on the first frame that is past its stated end time.
            //If the segment reported IsComplete, move on to the next one immediately (not next frame, now)
            if (currentSegment.IsComplete(span))
            {
                ResetViewport();
                SegmentNumber++;
                if (currentSegment == null)
                {
                    this.Exit();
                    return;
                }
                SegmentStartTime = VideoMode ? GetTimespan(FrameNumber, VideoFrameRate) : gameTime.TotalGameTime;
                span = TimeSpan.Zero;
            }

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            // Ask current segment if it is ready (done with any precalculating).  If not, wait.
            if (!currentSegment.IsReady)
            {
                SegmentStartTime = VideoMode ? GetTimespan(FrameNumber, VideoFrameRate) : gameTime.TotalGameTime;
                return;
            }

            currentSegment.Draw(span);

            base.Draw(gameTime);
            FrameNumber++;
        }

        public static TimeSpan GetTimespan(int FrameNumber, double VideoFrameRate)
        {
            //FromTicks is the only way to get a TimeSpan with fractional milliseconds.  FromMilliseconds truncates the input to an integral number of msec
            return TimeSpan.FromTicks((long)(FrameNumber / VideoFrameRate * 10000000D));
        }

        protected void ResetViewport()
        {
            var viewport = GraphicsDevice.Viewport;
            viewport.X = 0;
            viewport.Y = 0;
            viewport.Width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            viewport.Height = GraphicsDevice.PresentationParameters.BackBufferHeight;
            GraphicsDevice.Viewport = viewport;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            foreach (var segment in segments)
            {
                segment.AbortThreads();
            }

            base.OnExiting(sender, args);
        }
    }
}
