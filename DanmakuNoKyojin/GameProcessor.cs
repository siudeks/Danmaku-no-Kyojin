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
        Camera2D camera;


        public void LoadContent(IContentLoader provider)
        {
            Select = provider.Load<SoundEffect>(@"Audio/SE/select");
            Choose = provider.Load<SoundEffect>(@"Audio/SE/choose");

            camera = new Camera2D(viewport);

            _stateManager = new GameStateManager();

            // Screens
            TitleScreen = new TitleScreen(viewport, _stateManager, Select, Choose);
            contentBasedParts.Add(TitleScreen);

            GameConfigurationScreen = new GameConfigurationScreen(viewport, _stateManager);
            GameplayScreen = new GameplayScreen(viewport, _stateManager, camera); // .DisposeWith(instanceDisposer);
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
        }

        public void Update(GameTime gameTime)
        {
            foreach (var item in updatableParts)
                item.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (var item in drawableParts)
                item.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        public void Initialize()
        {
        }
    }
}
