using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ninject;
using System;
using System.Linq;

namespace DanmakuNoKyojin.Controls
{
    /// <summary>
    /// Manage inputs like keyboard or gamepad
    /// </summary>
    public sealed class InputHandler : IUpdatablePart, IInputHandler
    {
        [Inject]
        public IObserver<MouseState> MouseStateDispatcher { private get; set; }

        static MouseState mouseState = default(MouseState);
        static MouseState lastMouseState = default(MouseState);

        static KeyboardState keyboardState;
        static KeyboardState lastKeyboardState;

        static GamePadState[] gamePadStates;
        static GamePadState[] lastGamePadStates;

        public static MouseState MouseState
        {
            get { return mouseState; }
        }

        public static KeyboardState KeyboardState
        {
            get { return keyboardState; }
        }

        public static KeyboardState LastKeyboardState
        {
            get { return lastKeyboardState; }
        }

        public static GamePadState[] GamePadStates
        {
            get { return gamePadStates; }
        }

        public static GamePadState[] LastGamePadStates
        {
            get { return lastGamePadStates; }
        }

        static InputHandler()
        {
            gamePadStates = new GamePadState[Enum.GetValues(typeof(PlayerIndex)).Length];

            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                gamePadStates[(int)index] = GamePad.GetState(index);
        }

        public void Update(GameTime gameTime)
        {
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();
            MouseStateDispatcher.OnNext(mouseState);

            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            lastGamePadStates = (GamePadState[])gamePadStates.Clone();
            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                gamePadStates[(int)index] = GamePad.GetState(index);

        }

        public static void Flush()
        {
            lastMouseState = mouseState;
            lastKeyboardState = keyboardState;
        }

        public static bool PressedUp()
        {
            return
                (KeyPressed(Keys.Up) ||
                ButtonPressed(Buttons.DPadUp, PlayerIndex.One) ||
                ButtonPressed(Buttons.LeftThumbstickUp, PlayerIndex.One));
        }

        public static bool PressedDown()
        {
            return
                (KeyPressed(Keys.Down) ||
                ButtonPressed(Buttons.DPadDown, PlayerIndex.One) ||
                ButtonPressed(Buttons.LeftThumbstickDown, PlayerIndex.One));
        }

        public static bool PressedLeft()
        {
            return
                (KeyPressed(Keys.Left) ||
                ButtonPressed(Buttons.DPadLeft, PlayerIndex.One) ||
                ButtonPressed(Buttons.LeftThumbstickLeft, PlayerIndex.One));
        }

        public static bool PressedRight()
        {
            return
                (KeyPressed(Keys.Right) ||
                ButtonPressed(Buttons.DPadRight, PlayerIndex.One) ||
                ButtonPressed(Buttons.LeftThumbstickRight, PlayerIndex.One));
        }

        public static bool PressedAction()
        {
            return
                (KeyPressed(Keys.Enter) ||
                ButtonPressed(Buttons.A, PlayerIndex.One));
        }

        public static bool PressedCancel()
        {
            return
                (KeyPressed(Keys.Escape) ||
                ButtonPressed(Buttons.B, PlayerIndex.One));
        }

        public static bool Scroll()
        {
            return mouseState.ScrollWheelValue == lastMouseState.ScrollWheelValue;
        }

        public static bool ScrollUp()
        {
            return mouseState.ScrollWheelValue > lastMouseState.ScrollWheelValue;
        }

        public static bool ScrollDown()
        {
            return mouseState.ScrollWheelValue < lastMouseState.ScrollWheelValue;
        }

        public static bool KeyReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key) &&
                lastKeyboardState.IsKeyDown(key);
        }

        public static bool KeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) &&
                lastKeyboardState.IsKeyUp(key);
        }

        public static bool KeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        public static bool HavePressedKey()
        {
            return keyboardState != lastKeyboardState;
        }

        public static Keys[] GetPressedKeys()
        {
            return keyboardState.GetPressedKeys();
        }

        public static bool ButtonReleased(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonUp(button) &&
                lastGamePadStates[(int)index].IsButtonDown(button);
        }

        public static bool ButtonPressed(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button) &&
                lastGamePadStates[(int)index].IsButtonUp(button);
        }

        public static bool ButtonDown(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button);
        }

        public static bool HavePressedButton(PlayerIndex index)
        {
            return gamePadStates[(int)index] != lastGamePadStates[(int)index];
        }

        public static Buttons[] GetPressedButton(PlayerIndex index)
        {
            return Enum.GetValues(typeof(Buttons)).Cast<Buttons>().Where(button => ButtonPressed(button, index)).ToArray();
        }
    }
}
