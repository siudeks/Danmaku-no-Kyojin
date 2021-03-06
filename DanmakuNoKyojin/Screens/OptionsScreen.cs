﻿using DanmakuNoKyojin.Controls;
using DanmakuNoKyojin.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace DanmakuNoKyojin.Screens
{
    public sealed class OptionsScreen : GameScreen
    {
        private string _title;
        private readonly string[] _menuText;
        private int _menuIndex;

        private Texture2D _background;
        private SpriteFont _titleFont;

        private Texture2D _keyboardIcon;
        private Texture2D _gamepadIcon;

        private Point _menuStartCoord;

        private Texture2D _volumeBar;

        private readonly IViewportProvider viewport;
        private readonly Texture2D pixel;
        private readonly SoundEffect select;

        public OptionsScreen(IViewportProvider viewport, GameStateManager manager, Texture2D pixel, SoundEffect select)
            : base(manager)
        {

            this.viewport = viewport;
            this.pixel = pixel;
            this.select = select;

            _title = "Options";
            _menuText = new string[]
                {
                    "1P Control",
                    "2P Control",
                    "Sound volume",
                    "Music volume"
                };

            _menuStartCoord = new Point(
                viewport.Width / 2,
                viewport.Height / 2 - 100);
        }

        public override void Initialize()
        {
            base.Initialize();

            _menuIndex = 0;
        }

        public override void LoadContent(IContentLoader loader)
        {
            _background = loader.Load<Texture2D>("Graphics/Pictures/background");
            _keyboardIcon = loader.Load<Texture2D>("Graphics/Pictures/keyboard_icon");
            _gamepadIcon = loader.Load<Texture2D>("Graphics/Pictures/gamepad_icon");
            _volumeBar = pixel;

            _titleFont = loader.Load<SpriteFont>("Graphics/Fonts/TitleFont");

            base.LoadContent(loader);
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHandler.PressedCancel())
                StateManager.ChangeState(GameStateManager.State.TitleScreen);

            if (InputHandler.PressedUp())
            {
                _menuIndex--;

                if (_menuIndex < 0)
                    _menuIndex = _menuText.Length - 1;

                select.Play();
            }

            if (InputHandler.PressedDown())
            {
                _menuIndex = (_menuIndex + 1) % _menuText.Length;
                select.Play();
            }

            if (InputHandler.PressedLeft())
            {
                if (_menuIndex == 0 || _menuIndex == 1)
                {
                    if (Config.PlayersController[_menuIndex] == Config.Controller.Keyboard)
                    {
                        Config.PlayersController[_menuIndex] = Config.Controller.GamePad;
                        Config.PlayersController[(_menuIndex + 1) % 2] = Config.Controller.Keyboard;
                    }
                    else
                    {
                        Config.PlayersController[_menuIndex] = Config.Controller.Keyboard;
                        Config.PlayersController[(_menuIndex + 1) % 2] = Config.Controller.GamePad;
                    }
                }
                // Sound volume
                else if (_menuIndex == 2)
                {
                    if (Config.SoundVolume > 0)
                        Config.SoundVolume -= 1;

                    SoundEffect.MasterVolume = Config.SoundVolume/100f;

                }
                // Music volume
                else if (_menuIndex == 3)
                {
                    if (Config.MusicVolume > 0)
                        Config.MusicVolume -= 1;

                    MediaPlayer.Volume = Config.MusicVolume/100f;
                }

                select.Play();
            }

            if (InputHandler.PressedRight())
            {
                if (_menuIndex == 0 || _menuIndex == 1)
                {
                    if (Config.PlayersController[_menuIndex] == Config.Controller.Keyboard)
                    {
                        Config.PlayersController[_menuIndex] = Config.Controller.GamePad;
                        Config.PlayersController[(_menuIndex + 1) % 2] = Config.Controller.Keyboard;
                    }
                    else
                    {
                        Config.PlayersController[_menuIndex] = Config.Controller.Keyboard;
                        Config.PlayersController[(_menuIndex + 1) % 2] = Config.Controller.GamePad;
                    }
                }
                // Sound volume
                else if (_menuIndex == 2)
                {
                    if (Config.SoundVolume < 100)
                        Config.SoundVolume += 1;

                    SoundEffect.MasterVolume = Config.SoundVolume/100f;
                }
                // Music volume
                else if (_menuIndex == 3)
                {
                    if (Config.MusicVolume < 100)
                        Config.MusicVolume += 1;

                    MediaPlayer.Volume = Config.MusicVolume / 100f;
                }

                select.Play();
            }

            if (InputHandler.PressedAction())
            {
                /*
                if (_menuIndex == 0 || _menuIndex == 1)
                {
                    if (Config.PlayersController[_menuIndex] == Config.Controller.Keyboard)
                        StateManager.ChangeState(GameRef.KeyboardInputsScreen);
                    else
                        StateManager.ChangeState(GameRef.GamepadInputsScreen);

                }
                */
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // GameRef.SpriteBatch.Begin();

            spriteBatch.Draw(_background, new Rectangle(0, 0, Config.Resolution.X, Config.Resolution.Y), Color.Yellow);

            spriteBatch.DrawString(_titleFont, _title,
                new Vector2(
                    viewport.Width / 2f - _titleFont.MeasureString(_title).X / 2 + 5,
                    viewport.Height / 2f - (_titleFont.MeasureString(_title).Y * 2) + 5),
                Color.Black);
            spriteBatch.DrawString(_titleFont, _title,
                new Vector2(
                    viewport.Width / 2f - _titleFont.MeasureString(_title).X / 2,
                    viewport.Height / 2f - (_titleFont.MeasureString(_title).Y * 2)),
                Color.White);

            for (int i = 0; i < _menuText.Length; i++)
            {
                Color textColor = Color.White;

                if (i == _menuIndex)
                    textColor = Color.OrangeRed;

                spriteBatch.DrawString(ControlManager.SpriteFont, _menuText[i], new Vector2(
                  _menuStartCoord.X - (ControlManager.SpriteFont.MeasureString(_menuText[i]).X / 2f) + 1,
                  _menuStartCoord.Y + (100 * i) + 1), Color.Black);
                spriteBatch.DrawString(ControlManager.SpriteFont, _menuText[i], new Vector2(
                  _menuStartCoord.X - (ControlManager.SpriteFont.MeasureString(_menuText[i]).X / 2f),
                  _menuStartCoord.Y + (100 * i)), textColor);

                if (i == 0 || i == 1)
                {
                    Color keyboardColor = Color.White;
                    Color gamepadColor = Color.White;

                    if (Config.PlayersController[i] == Config.Controller.Keyboard)
                        keyboardColor = Color.Red;
                    if (Config.PlayersController[i] == Config.Controller.GamePad)
                        gamepadColor = Color.Red;

                    spriteBatch.Draw(_keyboardIcon, new Rectangle(
                        _menuStartCoord.X - 50 - 20, _menuStartCoord.Y + 35 + (i * 100),
                        _keyboardIcon.Width, _keyboardIcon.Height), keyboardColor);
                    spriteBatch.Draw(_gamepadIcon, new Rectangle(
                        _menuStartCoord.X + 50 - 20, _menuStartCoord.Y + 35 + (i * 100),
                        _gamepadIcon.Width, _gamepadIcon.Height), gamepadColor);
                }

                // Sound volume
                if (i == 2)
                {
                    spriteBatch.Draw(_volumeBar, new Rectangle(
                       _menuStartCoord.X - 60, _menuStartCoord.Y + (100 * i) + 35,
                        (int)(150f * (Config.SoundVolume / 100f)), 20),
                    Color.White);


                    string soundVolume = Config.SoundVolume.ToString() + "%";
                    spriteBatch.DrawString(ControlManager.SpriteFont, soundVolume,
                        new Vector2(_menuStartCoord.X - ControlManager.SpriteFont.MeasureString(soundVolume).X / 4 + 1, _menuStartCoord.Y + (100 * i) + 60 + 1),
                        Color.Black);

                    spriteBatch.DrawString(ControlManager.SpriteFont, soundVolume,
                        new Vector2(_menuStartCoord.X - ControlManager.SpriteFont.MeasureString(soundVolume).X / 4, _menuStartCoord.Y + (100 * i) + 60),
                        Color.White);
                }
                // Music volume
                else if (i == 3)
                {
                    spriteBatch.Draw(_volumeBar, new Rectangle(
                       _menuStartCoord.X - 60, _menuStartCoord.Y + (100 * i) + 35,
                        (int)(150f * (Config.MusicVolume/100f)), 20),
                    Color.White);


                    string musicVolume = Config.MusicVolume.ToString() + "%";
                    spriteBatch.DrawString(ControlManager.SpriteFont, musicVolume,
                        new Vector2(_menuStartCoord.X - ControlManager.SpriteFont.MeasureString(musicVolume).X / 4 + 1, _menuStartCoord.Y + (100 * i) + 60 + 1),
                        Color.Black);

                    spriteBatch.DrawString(ControlManager.SpriteFont, musicVolume,
                        new Vector2(_menuStartCoord.X - ControlManager.SpriteFont.MeasureString(musicVolume).X / 4, _menuStartCoord.Y + (100 * i) + 60),
                        Color.White);
                }
            }

            //GameRef.SpriteBatch.End();

            base.Draw(gameTime, spriteBatch);
        }
    }
}
