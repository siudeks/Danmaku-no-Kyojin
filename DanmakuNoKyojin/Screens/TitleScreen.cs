using System.Globalization;
using DanmakuNoKyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using DanmakuNoKyojin.Framework;
using System.Diagnostics;

namespace DanmakuNoKyojin.Screens
{
    public class TitleScreen : GameScreen
    {
        #region Field region

        private Texture2D _logo;
        private readonly string[] _menuText;
        private readonly string[] _menuDescription;
        private int _menuIndex;
        private int _passStep;

        private Texture2D _backgroundImage;
        private Rectangle _backgroundMainRectangle;
        private Rectangle _backgroundRightRectangle;
        private Rectangle _backgroundTopRectangle;
        private Rectangle _backgroundTopRightRectangle;

        // Audio
        private SoundEffect _passSound = null;

        #endregion

        #region Constructor region

        private readonly IViewportProvider viewport;
        private readonly SoundEffect select;
        private readonly SoundEffect choose;

        public TitleScreen(IViewportProvider viewport,  GameStateManager manager, SoundEffect select, SoundEffect choose)
            : base(manager)
        {
            this.viewport = viewport;
            this.select = select;
            this.choose = choose;

            _menuText = new string[] { "Start", "Shop", "Options", "Exit" };
            _menuDescription = new string[] { 
                "Playing game with only one player", 
                "Get new abilities to crush more enemies",
                "You can change inputs here", 
                "Warning: I've never tested this button !", 
            };

            _menuIndex = 0;
        }

        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            _backgroundMainRectangle = new Rectangle(0, 0, Config.Resolution.X, Config.Resolution.Y);
            _backgroundRightRectangle = new Rectangle(Config.Resolution.X, 0, Config.Resolution.X, Config.Resolution.Y);
            _backgroundTopRectangle = new Rectangle(0, -Config.Resolution.Y, Config.Resolution.X, Config.Resolution.Y);
            _backgroundTopRightRectangle = new Rectangle(Config.Resolution.X, -Config.Resolution.Y, Config.Resolution.X, Config.Resolution.Y);


            SoundEffect.MasterVolume = Config.SoundVolume/100f;

            base.Initialize();
        }

        public override void LoadContent(IContentLoader loader)
        {
            // Music
            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Volume = Config.MusicVolume / 100f;
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(loader.Load<Song>("Audio/Musics/menu"));
            }

            _logo = loader.Load<Texture2D>("Graphics/Pictures/logo");
            _backgroundImage = loader.Load<Texture2D>("Graphics/Pictures/background");

            if (_passSound == null)
                _passSound = loader.Load<SoundEffect>(@"Audio/SE/pass");

            base.LoadContent(loader);
        }

        public override void Update(GameTime gameTime)
        {
            //if (InputHandler.PressedCancel() && _passStep != 8)
            //    Game.Exit();

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

            if (InputHandler.PressedAction() && _passStep != 9)
            {
                choose.Play();

                // 1 Player
                if (_menuIndex == 0)
                {
                    StateManager.ChangeState(GameStateManager.State.GameplayScreen);
                }
                // Improvements
                else if (_menuIndex == 1)
                    StateManager.ChangeState(GameStateManager.State.ImprovementScreen);
                // Options
                else if (_menuIndex == 2)
                    StateManager.ChangeState(GameStateManager.State.OptionsScreen);
                // Exit
                else if (_menuIndex == 3)
                    Process.GetCurrentProcess().Kill();
            }

            if (_backgroundMainRectangle.X + Config.Resolution.X <= 0)
                _backgroundMainRectangle.X = _backgroundRightRectangle.X + Config.Resolution.X;

            if (_backgroundRightRectangle.X + Config.Resolution.X <= 0)
                _backgroundRightRectangle.X = _backgroundMainRectangle.X + Config.Resolution.X;

            /*
            if (_backgroundMainRectangle.Y + _backgroundImage.Height <= 0)
                _backgroundMainRectangle.Y = _backgroundTopRectangle.Y - _backgroundImage.Height;

            if (_backgroundTopRectangle.Y + _backgroundImage.Height <= 0)
                _backgroundTopRectangle.Y = _backgroundMainRectangle.Y - _backgroundImage.Height;
            */


            _backgroundMainRectangle.X -= (int)(70 * (float)gameTime.ElapsedGameTime.TotalSeconds);
            _backgroundRightRectangle.X -= (int)(70 * (float)gameTime.ElapsedGameTime.TotalSeconds);

            // Cheat and debug mode
            if (!Config.Cheat)
            {
                if ((_passStep == 0 || _passStep == 1) && InputHandler.PressedUp())
                {
                    _passStep++;
                }
                else if ((_passStep == 2 || _passStep == 3) && InputHandler.PressedDown())
                {
                    _passStep++;
                }
                else if ((_passStep == 4 || _passStep == 6) && InputHandler.PressedLeft())
                {
                    _passStep++;
                }
                else if ((_passStep == 5 || _passStep == 7) && InputHandler.PressedRight())
                {
                    _passStep++;
                }
                else if (_passStep == 8 && (InputHandler.PressedCancel() || InputHandler.KeyPressed(Keys.B)))
                {
                    _passStep++;
                }
                else if (_passStep == 9 && (InputHandler.PressedAction() || InputHandler.KeyPressed(Keys.A)))
                {
                    _passStep++;
                }

                if (_passStep == 10)
                {
                    Config.Cheat = true;
                    _passSound.Play();
                }
            }
            else if (InputHandler.KeyDown(Keys.F9))
            {
                Config.Debug = !Config.Debug;
            }
            
            if (Config.Cheat)
            {
                if (InputHandler.KeyDown(Keys.A) || InputHandler.ButtonDown(Buttons.Y, PlayerIndex.One))
                {
                    PlayerData.Credits += 1000;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // GameRef.SpriteBatch.Begin();

            spriteBatch.Draw(_backgroundImage, _backgroundMainRectangle, Color.White);
            spriteBatch.Draw(_backgroundImage, _backgroundRightRectangle, Color.White);
            spriteBatch.Draw(_backgroundImage, _backgroundTopRectangle, Color.White);
            spriteBatch.Draw(_backgroundImage, _backgroundTopRightRectangle, Color.White);

            spriteBatch.Draw(_logo, new Vector2(
                                                (viewport.Width / 2) - (_logo.Width / 2),
                                                100), Color.White);

            for (int i = 0; i < _menuText.Length; i++)
            {
                Color textColor = Color.White;

                if (i == _menuIndex)
                    textColor = Color.OrangeRed;

                spriteBatch.DrawString(ControlManager.SpriteFont, _menuText[i], new Vector2(
                  viewport.Width / 2f - (ControlManager.SpriteFont.MeasureString(_menuText[i]).X / 2f) + 1,
                  viewport.Height / 2f + (50 * i) + 1), Color.Black);
                spriteBatch.DrawString(ControlManager.SpriteFont, _menuText[i], new Vector2(
                  viewport.Width / 2f - (ControlManager.SpriteFont.MeasureString(_menuText[i]).X / 2f),
                  viewport.Height / 2f + (50 * i)), textColor);
            }

            spriteBatch.DrawString(ControlManager.SpriteFont, "[" + _menuDescription[_menuIndex] + "]", new Vector2(
                viewport.Width / 2f - (ControlManager.SpriteFont.MeasureString(_menuDescription[_menuIndex]).X / 2f) - 4 + 1,
                viewport.Height - 60 + 1), Color.Black);
            spriteBatch.DrawString(ControlManager.SpriteFont, "[" + _menuDescription[_menuIndex] + "]", new Vector2(
                viewport.Width / 2f - (ControlManager.SpriteFont.MeasureString(_menuDescription[_menuIndex]).X / 2f) - 4,
                viewport.Height - 60), Color.White);

            string credits = "Credits: " + PlayerData.Credits.ToString(CultureInfo.InvariantCulture);

            spriteBatch.DrawString(ControlManager.SpriteFont, credits, new Vector2(1, viewport.Height - ControlManager.SpriteFont.MeasureString(credits).Y + 1), Color.Black);
            spriteBatch.DrawString(ControlManager.SpriteFont, credits, new Vector2(0, viewport.Height - ControlManager.SpriteFont.MeasureString(credits).Y), Color.White);

            //GameRef.SpriteBatch.End();

            base.Draw(gameTime, spriteBatch);
        }

        #endregion
    }
}
