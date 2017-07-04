using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DanmakuNoKyojin.Controls
{
    public sealed class GameStateManager
    {
        const int startDrawOrder = 5000;
        const int drawOrderInc = 100;

        public event EventHandler<IGameComponent> ComponentAdded;
        public event EventHandler<IGameComponent> ComponentRemoved;

        private readonly Stack<GameState> gameStates = new Stack<GameState>();

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

        private void AddState(GameState newState)
        {
            gameStates.Push(newState);

            ComponentAdded?.Invoke(this, newState);
        }

        public void ChangeState(GameState newState)
        {
            while (gameStates.Count > 0)
                RemoveState();

            newState.DrawOrder = startDrawOrder;

            AddState(newState);
        }
    }
}
