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

namespace DanmakuNoKyojin
{
    public class GameRunner : Game, IContentLoader
    {
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        private GameStateManager _stateManager;

        // single instance to dispose all disposable resources owned by GameProcessor
        private CompositeDisposable instanceDisposer = new CompositeDisposable();

        [Inject]
        public GameProcessor GameProcessor { private get; set; }

        // Screens
        public TitleScreen TitleScreen;
        public DebugScreen DebugScreen;
        public PatternTestScreen PatternTestScreen;
        public ImprovementScreen ImprovementScreen;
        public LeaderboardScreen LeaderboardScreen;
        public OptionsScreen OptionsScreen;
        public KeyboardInputsScreen KeyboardInputsScreen;
        public GamepadInputsScreen GamepadInputsScreen;
        public GameConfigurationScreen GameConfigurationScreen;
        public GameplayScreen GameplayScreen;
        public GameOverScreen GameOverScreen;

        public Rectangle ScreenRectangle;

        // Pause
        public bool Pause;

        // Useful pixel
        public readonly Texture2D Pixel;

        // Particles
        public ParticleManager<ParticleState> ParticleManager { get; private set; }
        public Texture2D LineParticle;
        public Texture2D Glow;

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
            StaticClassSerializer.Load(typeof(PlayerData), "data.bin");

            // Display FPS at the top left screen's corner
            Components.Add(new FrameRateCounter(this));

            _stateManager = new GameStateManager();
            _stateManager.ComponentAdded += (s, arg) => Components.Add(arg);
            _stateManager.ComponentRemoved += (s, arg) => Components.Remove(arg);

            // Screens
            TitleScreen = new TitleScreen(this, _stateManager).DisposeWith(instanceDisposer);
            DebugScreen = new DebugScreen(this, _stateManager).DisposeWith(instanceDisposer);
            PatternTestScreen = new PatternTestScreen(this, _stateManager).DisposeWith(instanceDisposer);
            GameConfigurationScreen = new GameConfigurationScreen(this, _stateManager).DisposeWith(instanceDisposer);
            GameplayScreen = new GameplayScreen(this, _stateManager).DisposeWith(instanceDisposer);
            LeaderboardScreen = new LeaderboardScreen(this, _stateManager).DisposeWith(instanceDisposer);
            ImprovementScreen = new ImprovementScreen(this, _stateManager).DisposeWith(instanceDisposer);
            GameOverScreen = new GameOverScreen(this, _stateManager).DisposeWith(instanceDisposer);
            OptionsScreen = new OptionsScreen(this, _stateManager).DisposeWith(instanceDisposer);
            KeyboardInputsScreen = new KeyboardInputsScreen(this, _stateManager).DisposeWith(instanceDisposer);
            GamepadInputsScreen = new GamepadInputsScreen(this, _stateManager).DisposeWith(instanceDisposer);

            _stateManager.ChangeState(TitleScreen);

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
            SpriteBatch = new SpriteBatch(GraphicsDevice).DisposeWith(instanceDisposer);

            Select = Content.Load<SoundEffect>(@"Audio/SE/select");
            Choose = Content.Load<SoundEffect>(@"Audio/SE/choose");

            LineParticle = Content.Load<Texture2D>("Graphics/Pictures/laser");
            Glow = Content.Load<Texture2D>("Graphics/Pictures/glow");
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

            base.Draw(gameTime);
        }

        public T Load<T>(string assetName)
        {
            return Content.Load<T>(assetName);
        }
    }
}
