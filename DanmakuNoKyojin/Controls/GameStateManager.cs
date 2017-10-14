using DanmakuNoKyojin.Screens;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DanmakuNoKyojin.Controls
{
    public sealed class GameStateManager
    {
        const int startDrawOrder = 5000;
        const int drawOrderInc = 100;

        public event EventHandler<GameState> ComponentAdded;
        public event EventHandler<GameState> ComponentRemoved;

        private readonly Stack<GameState> gameStates = new Stack<GameState>();
        private readonly Dictionary<State, GameScreen> screens = new Dictionary<State, GameScreen>();

        public void AddScreens(GameOverScreen GameOverScreen,
            OptionsScreen OptionsScreen,
            TitleScreen TitleScreen,
            GameplayScreen GameplayScreen,
            ImprovementScreen ImprovementScreen)
        {
            screens.Add(State.GameOverScreen, GameOverScreen);
            screens.Add(State.OptionsScreen, OptionsScreen);
            screens.Add(State.TitleScreen, TitleScreen);
            screens.Add(State.GameplayScreen, GameplayScreen);
            screens.Add(State.ImprovementScreen, ImprovementScreen);
        }
        public GameState CurrentState
        {
            get { return gameStates.Peek(); }
        }

        private void RemoveState()
        {
            GameState State = gameStates.Peek();

            ComponentRemoved?.Invoke(this, State);
            gameStates.Pop();
        }

        private void AddState(State state)
        {
            var screen = screens[state];
            gameStates.Push(screen);

            ComponentAdded?.Invoke(this, screen);
        }

        public void ChangeState(State expected)
        {
            var newState = screens[expected];
            while (gameStates.Count > 0)
                RemoveState();

            //newState.DrawOrder = startDrawOrder;

            AddState(expected);
        }

        public enum State
        {
            GameOverScreen,
            OptionsScreen,
            TitleScreen,
            GameplayScreen,
            ImprovementScreen
        }
    }
}
