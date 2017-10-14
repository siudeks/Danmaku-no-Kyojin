using DanmakuNoKyojin.Camera;
using DanmakuNoKyojin.Controls;
using DanmakuNoKyojin.Framework;
using DanmakuNoKyojin.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Ninject;
using System.Collections.Generic;

namespace DanmakuNoKyojin
{
    /// <summary>
    /// Main game executor.
    /// 
    /// Handles game events and distribute them to parts of the game.
    /// Responsibility of the class is similar to XNA Game, but it has been created with testability in mind.
    /// </summary>
    public sealed class GameProcessor : IInitializable
    {
        // Screens
        public TitleScreen TitleScreen;
        public ImprovementScreen ImprovementScreen;
        public LeaderboardScreen LeaderboardScreen;
        public OptionsScreen OptionsScreen;
        public KeyboardInputsScreen KeyboardInputsScreen;
        public GamepadInputsScreen GamepadInputsScreen;
        public GameConfigurationScreen GameConfigurationScreen;
        public GameplayScreen GameplayScreen;
        public GameOverScreen GameOverScreen;

        [Inject] public IUpdatablePart[] UpdatableParts { private get; set; }
        [Inject] public IDrawablePart[] DrawableParts { private get; set; }
        [Inject] public IContentBasedPart[] ContentBasedParts { private get; set; }
        [Inject] public InputHandler InputHandler { private get; set; }

        private List<IUpdatablePart> updatableParts = new List<IUpdatablePart>();
        private List<IDrawablePart> drawableParts = new List<IDrawablePart>();
        private List<IContentBasedPart> contentBasedParts = new List<IContentBasedPart>();

        private GameStateManager _stateManager;
        private readonly IViewportProvider viewport;
        private readonly Texture2D pixel;
        public GameProcessor(IViewportProvider viewport, Texture2D pixel)
        {
            this.viewport = viewport;
            this.pixel = pixel;
        }

        private SoundEffect Select;
        private SoundEffect Choose;

        [Inject] public Camera2D Camera { private get; set; }

        public void LoadContent(IContentLoader provider)
        {
            Select = provider.Load<SoundEffect>(@"Audio/SE/select");
            Choose = provider.Load<SoundEffect>(@"Audio/SE/choose");

            _stateManager = new GameStateManager();

            // Screens
            TitleScreen = new TitleScreen(viewport, _stateManager, Select, Choose, InputHandler);
            contentBasedParts.Add(TitleScreen);

            GameConfigurationScreen = new GameConfigurationScreen(viewport, _stateManager);
            GameplayScreen = new GameplayScreen(viewport, _stateManager, Camera); // .DisposeWith(instanceDisposer);
            GameplayScreen.Initialize();
            contentBasedParts.Add(GameplayScreen);

            LeaderboardScreen = new LeaderboardScreen(viewport, _stateManager);
            ImprovementScreen = new ImprovementScreen(viewport, _stateManager, Select, Choose);
            GameOverScreen = new GameOverScreen(viewport, _stateManager);
            OptionsScreen = new OptionsScreen(viewport, _stateManager, pixel, Select);
            KeyboardInputsScreen = new KeyboardInputsScreen(viewport, _stateManager);
            GamepadInputsScreen = new GamepadInputsScreen(viewport, _stateManager);

            _stateManager.AddScreens(GameOverScreen, OptionsScreen, TitleScreen, GameplayScreen, ImprovementScreen);

            _stateManager.ComponentAdded += (s, arg) =>
            {
                updatableParts.Add(arg);
                drawableParts.Add(arg);
            };
            _stateManager.ComponentRemoved += (s, arg) =>
            {
                updatableParts.Remove(arg);
                drawableParts.Remove(arg);
            };

            _stateManager.ChangeState(GameStateManager.State.TitleScreen);


            foreach (var item in contentBasedParts)
                item.LoadContent(provider);

            foreach (var item in ContentBasedParts)
                item.LoadContent(provider);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var item in UpdatableParts)
            {
                item.Update(gameTime);
            }

            foreach (var item in updatableParts.ToArray())
                item.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var item in drawableParts)
                item.Draw(gameTime, spriteBatch);

            foreach (var item in DrawableParts)
                item.Draw(gameTime, spriteBatch);
        }

        public void Initialize()
        {
        }
    }
}
