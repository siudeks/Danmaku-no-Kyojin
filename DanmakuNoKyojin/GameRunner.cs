using System;
using DanmakuNoKyojin.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using DanmakuNoKyojin.Controls;
using DanmakuNoKyojin.Screens;
using DanmakuNoKyojin.Utils;
using Microsoft.Xna.Framework.Input;
using System.Reactive.Disposables;
using DanmakuNoKyojin.Framework;
using Ninject;
using DanmakuNoKyojin.Camera;

namespace DanmakuNoKyojin
{
    public sealed class GameRunner : Game, IContentLoader, IViewportProvider
    {
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;

        // single instance to dispose all disposable resources owned by GameProcessor
        private CompositeDisposable instanceDisposer = new CompositeDisposable();

        public GameProcessor GameProcessor { private get; set; }

        public Rectangle ScreenRectangle;

        // Pause
        public bool Pause;

        // Useful pixel
        public readonly Texture2D Pixel;

        // Particles
        public ParticleManager<ParticleState> ParticleManager { get; private set; }

        int IViewportProvider.Width => Graphics.GraphicsDevice.Viewport.Width;

        int IViewportProvider.Height => Graphics.GraphicsDevice.Viewport.Height;


        // Audio
        public SoundEffect Select;
        public SoundEffect Choose;

        public GameRunner()
        {
            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Config.Resolution.X,
                PreferredBackBufferHeight = Config.Resolution.Y
            };

            ScreenRectangle = new Rectangle(0, 0, Config.Resolution.X, Config.Resolution.Y);
            IsMouseVisible = true;
            Graphics.IsFullScreen = Config.FullScreen;
            Graphics.SynchronizeWithVerticalRetrace = true;

            // Pass through the FPS capping (60 FPS)
            if (!Config.FpsCapping)
            {
                IsFixedTimeStep = false;
                Graphics.SynchronizeWithVerticalRetrace = false;
            }

            Graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });
        }

        protected override void Initialize()
        {
            //StaticClassSerializer.Load(typeof(PlayerData), "data.bin");

            // Display FPS at the top left screen's corner
            Components.Add(new FrameRateCounter(this));





            ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                instanceDisposer.Dispose();
                StaticClassSerializer.Save(typeof(PlayerData), "data.bin");
            }

            base.Dispose(disposing);
        }

        protected override void LoadContent()
        {
            GameProcessor.LoadContent(this);

            SpriteBatch = new SpriteBatch(GraphicsDevice).DisposeWith(instanceDisposer);

            ParticleManager.LoadContent(this);
        }


        protected override void Update(GameTime gameTime)
        {
            GameProcessor.Update(gameTime);

            if (InputHandler.KeyPressed(Keys.F1) || InputHandler.ButtonPressed(Buttons.Start, PlayerIndex.One))
            {
                Config.FullScreen = !Config.FullScreen;
                Graphics.IsFullScreen = Config.FullScreen;
                Graphics.ApplyChanges();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GameProcessor.Draw(gameTime, SpriteBatch);

            base.Draw(gameTime);
        }

        public T Load<T>(string assetName)
        {
            return Content.Load<T>(assetName);
        }
    }
}
