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
        private int drawOrder = startDrawOrder;

        public GameState CurrentState
        {
            get { return gameStates.Peek(); }
        }

        public void PopState()
        {
            if (gameStates.Count > 0)
            {
                RemoveState();
                drawOrder -= drawOrderInc;
            }
        }

        private void RemoveState()
        {
            GameState State = gameStates.Peek();

            ComponentRemoved?.Invoke(this, State);
            gameStates.Pop();
        }

        public void PushState(GameState newState)
        {
            drawOrder += drawOrderInc;
            newState.DrawOrder = drawOrder;

            AddState(newState);
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
            drawOrder = startDrawOrder;

            AddState(newState);
        }
    }
}
