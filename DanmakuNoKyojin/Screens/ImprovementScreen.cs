﻿using System.Collections.Generic;
using System.Globalization;
using DanmakuNoKyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using DanmakuNoKyojin.Framework;

namespace DanmakuNoKyojin.Screens
{
    public class ImprovementScreen : GameScreen
    {
        #region Field region

        private string _title;
        private string _credits;
        private string _error;
        private string[] _menuText;
        private int _menuIndex;
        private Dictionary<string, bool> _finished;
        private Texture2D _background;
        private SpriteFont _titleFont;

        private SoundEffect _buySound;

        #endregion

        private readonly IViewportProvider viewport;
        private readonly SoundEffect select;
        private readonly SoundEffect choose;

        public ImprovementScreen(IViewportProvider viewport, GameStateManager manager, SoundEffect select, SoundEffect choose)
            : base(manager)
        {
            this.viewport = viewport;
            this.select = select;
            this.choose = choose;
        }

        #region XNA Method region

        public override void Initialize()
        {
            _title = "Shop";

            UpdateMenuText();

            _menuIndex = 0;

            base.Initialize();
        }

        public override void LoadContent(IContentLoader loader)
        {
            _background = loader.Load<Texture2D>("Graphics/Pictures/background");

            _titleFont = loader.Load<SpriteFont>("Graphics/Fonts/TitleFont");

            _buySound = loader.Load<SoundEffect>(@"Audio/SE/buy");

            base.LoadContent(loader);
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHandler.PressedCancel())
                StateManager.ChangeState(GameStateManager.State.TitleScreen);

            if (InputHandler.PressedUp())
            {
                select.Play();

                _menuIndex--;

                if (_menuIndex < 0)
                    _menuIndex = _menuText.Length;
            }

            if (InputHandler.PressedDown())
            {
                select.Play();

                _menuIndex = (_menuIndex + 1) % (_menuText.Length + 1);
            }

            if (InputHandler.PressedAction())
            {
                if (_menuIndex == _menuText.Length)
                {
                    choose.Play();
                    StateManager.ChangeState(GameStateManager.State.TitleScreen);
                }

                bool error = false;
                switch (_menuIndex)
                {
                    // Lives
                    case 0:
                        if (!_finished["livesNumber"] &&
                            PlayerData.LivesNumberIndex < Improvements.LivesNumberData.Count - 1 &&
                            PlayerData.Credits >= Improvements.LivesNumberData[PlayerData.LivesNumberIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.LivesNumberData[PlayerData.LivesNumberIndex + 1].Value;
                            PlayerData.LivesNumberIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 1:
                        if (!_finished["shootType"] &&
                            PlayerData.ShootTypeIndex < Improvements.ShootTypeData.Count - 1 &&
                            PlayerData.Credits >= Improvements.ShootTypeData[PlayerData.ShootTypeIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.ShootTypeData[PlayerData.ShootTypeIndex + 1].Value;
                            PlayerData.ShootTypeIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 2:
                        if (!_finished["shootPower"] &&
                            PlayerData.ShootPowerIndex < Improvements.ShootPowerData.Count - 1 &&
                            PlayerData.Credits >= Improvements.ShootPowerData[PlayerData.ShootPowerIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.ShootPowerData[PlayerData.ShootPowerIndex + 1].Value;
                            PlayerData.ShootPowerIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 3:
                        if (!_finished["shootFrequency"] &&
                            PlayerData.ShootFrequencyIndex < Improvements.ShootFrequencyData.Count - 1 &&
                            PlayerData.Credits >= Improvements.ShootFrequencyData[PlayerData.ShootFrequencyIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.ShootFrequencyData[PlayerData.ShootFrequencyIndex + 1].Value;
                            PlayerData.ShootFrequencyIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 4:
                        if (!_finished["timerInitialTime"] &&
                            PlayerData.TimerInitialTimeIndex < Improvements.TimerInitialTimeData.Count - 1 &&
                            PlayerData.Credits >= Improvements.TimerInitialTimeData[PlayerData.TimerInitialTimeIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.TimerInitialTimeData[PlayerData.TimerInitialTimeIndex + 1].Value;
                            PlayerData.TimerInitialTimeIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 5:
                        if (!_finished["timerExtraTime"] &&
                            PlayerData.TimerExtraTimeIndex < Improvements.TimerExtraTimeData.Count - 1 &&
                            PlayerData.Credits >= Improvements.TimerExtraTimeData[PlayerData.TimerExtraTimeIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.TimerExtraTimeData[PlayerData.TimerExtraTimeIndex + 1].Value;
                            PlayerData.TimerExtraTimeIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 6:
                        if (!_finished["bulletTimeDivisor"] &&
                            PlayerData.InvicibleTimeIndex < Improvements.InvicibleTimeData.Count - 1 &&
                            PlayerData.Credits >= Improvements.InvicibleTimeData[PlayerData.InvicibleTimeIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.InvicibleTimeData[PlayerData.InvicibleTimeIndex + 1].Value;
                            PlayerData.InvicibleTimeIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 8:
                        if (!_finished["bulletTime"] &&
                            !PlayerData.BulletTimeEnabled &&
                            PlayerData.Credits >= Improvements.BulletTimePrice)
                        {
                            PlayerData.Credits -= Improvements.BulletTimePrice;
                            PlayerData.BulletTimeEnabled = true;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 9:
                        if (!_finished["bulletTimeTimer"] &&
                            PlayerData.BulletTimeTimerIndex < Improvements.BulletTimeTimerData.Count - 1 &&
                            PlayerData.Credits >= Improvements.BulletTimeTimerData[PlayerData.BulletTimeTimerIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.BulletTimeTimerData[PlayerData.BulletTimeTimerIndex + 1].Value;
                            PlayerData.BulletTimeTimerIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 10:
                        if (!_finished["bulletTimeDivisor"] &&
                            PlayerData.BulletTimeDivisorIndex < Improvements.BulletTimeDivisorData.Count - 1 &&
                            PlayerData.Credits >= Improvements.BulletTimeDivisorData[PlayerData.BulletTimeDivisorIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.BulletTimeDivisorData[PlayerData.BulletTimeDivisorIndex + 1].Value;
                            PlayerData.BulletTimeDivisorIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;

                    default:
                        error = true;
                        break;
                }

                if (error)
                    _error = "You don't have enought credits to buy this !";
                else
                {
                    _error = "";
                    _buySound.Play();
                }

                UpdateMenuText();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // GameRef.SpriteBatch.Begin();

            spriteBatch.Draw(_background, new Rectangle(0, 0, Config.Resolution.X, Config.Resolution.Y), Color.GreenYellow);

            // Title
            spriteBatch.DrawString(_titleFont, _title,
                new Vector2(
                    viewport.Width / 2f - _titleFont.MeasureString(_title).X / 2 + 5,
                    viewport.Height / 2f - (_titleFont.MeasureString(_title).Y * 3) + 5),
                Color.Black);
            spriteBatch.DrawString(_titleFont, _title,
                new Vector2(
                    viewport.Width / 2f - _titleFont.MeasureString(_title).X / 2,
                    viewport.Height / 2f - (_titleFont.MeasureString(_title).Y * 3)),
                Color.White);

            // Credits
            spriteBatch.DrawString(ControlManager.SpriteFont, _credits,
                new Vector2(
                    viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(_credits).X / 2 + 1,
                    100 + 1),
                Color.Black);
            spriteBatch.DrawString(ControlManager.SpriteFont, _credits,
                new Vector2(
                    viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(_credits).X / 2,
                    100), 
                Color.White);

            var menuTextOrigin = new Point(300, 150);
            int lineHeight = 40;
            Color color;
            for (int i = 0; i < _menuText.Length; i++)
            {
                color = Color.White;
                spriteBatch.DrawString(ControlManager.SpriteFont, _menuText[i], new Vector2(menuTextOrigin.X + 1, menuTextOrigin.Y + lineHeight * i + 1), Color.Black);
                spriteBatch.DrawString(ControlManager.SpriteFont, _menuText[i], new Vector2(menuTextOrigin.X, menuTextOrigin.Y + lineHeight * i), Color.White);

                if (_menuIndex == i)
                    color = Color.Red;

                spriteBatch.DrawString(ControlManager.SpriteFont, "Buy", new Vector2(menuTextOrigin.X + 500 + 1, menuTextOrigin.Y + lineHeight * i + 1), Color.Black);
                spriteBatch.DrawString(ControlManager.SpriteFont, "Buy", new Vector2(menuTextOrigin.X + 500, menuTextOrigin.Y + lineHeight * i), color);
            }

            // Back
            color = Color.White;
            if (_menuIndex == _menuText.Length)
                color = Color.Red;

            string back = "Back to title";

            spriteBatch.DrawString(ControlManager.SpriteFont, back, new Vector2(
                Config.Resolution.X / 2f - ControlManager.SpriteFont.MeasureString(back).X / 2 + 1, 
                menuTextOrigin.Y + lineHeight * _menuText.Length + 1), 
            Color.Black);
            spriteBatch.DrawString(ControlManager.SpriteFont, back, new Vector2(
                Config.Resolution.X / 2f - ControlManager.SpriteFont.MeasureString(back).X / 2,
                menuTextOrigin.Y + lineHeight * _menuText.Length), 
            color);

            // Errors
            if (_error != null)
            {
                spriteBatch.DrawString(ControlManager.SpriteFont, _error,
                                               new Vector2(
                                                   viewport.Width/2f -
                                                   ControlManager.SpriteFont.MeasureString(_error).X/2 + 1,
                                                   Config.Resolution.Y - 40 + 1),
                                               Color.Black);
                spriteBatch.DrawString(ControlManager.SpriteFont, _error,
                                               new Vector2(
                                                   viewport.Width/2f -
                                                   ControlManager.SpriteFont.MeasureString(_error).X/2,
                                                   Config.Resolution.Y - 40),
                                               Color.White);
            }

            // GameRef.SpriteBatch.End();

            base.Draw(gameTime, spriteBatch);
        }

        #endregion

        private void UpdateMenuText()
        {
            _credits = "You currently have " + PlayerData.Credits.ToString(CultureInfo.InvariantCulture) + "$";

            _finished = new Dictionary<string, bool>();

            string livesNumber = "Lives numbers: ";
            if (PlayerData.LivesNumberIndex < Improvements.LivesNumberData.Count - 1)
            {
                livesNumber += (Improvements.LivesNumberData[PlayerData.LivesNumberIndex + 1].Key + " (" +
                                 Improvements.LivesNumberData[PlayerData.LivesNumberIndex + 1].Value + "$)");

                _finished.Add("livesNumber", false);
            }
            else
            {
                _finished.Add("livesNumber", true);
                livesNumber += "FINISHED";
            }

            string shootType = "Shoot type: ";
            if (PlayerData.ShootTypeIndex < Improvements.ShootTypeData.Count - 1)
            {
                shootType += Improvements.ShootTypeData[PlayerData.ShootTypeIndex + 1].Key + " (" +
                              Improvements.ShootTypeData[PlayerData.ShootTypeIndex + 1].Value + "$)";
                _finished.Add("shootType", false);
            }
            else
            {
                _finished.Add("shootType", true);
                shootType += "FINISHED";
            }

            string shootPower = "Shoot power: ";
            if (PlayerData.ShootPowerIndex < Improvements.ShootPowerData.Count - 1)
            {
                shootPower += "x" + Improvements.ShootPowerData[PlayerData.ShootPowerIndex + 1].Key + " (" +
                              Improvements.ShootPowerData[PlayerData.ShootPowerIndex + 1].Value + "$)";
                _finished.Add("shootPower", false);
            }
            else
            {
                _finished.Add("shootPower", true);
                shootPower += "FINISHED";
            }

            string shootFrequency = "Shoot frequency: ";
            if (PlayerData.ShootFrequencyIndex < Improvements.ShootFrequencyData.Count - 1)
            {
                shootFrequency += "x" + Improvements.ShootFrequencyData[PlayerData.ShootFrequencyIndex + 1].Key + " (" +
                                  Improvements.ShootFrequencyData[PlayerData.ShootFrequencyIndex + 1].Value + "$)";
                _finished.Add("shootFrequency", false);
            }
            else
            {
                _finished.Add("shootFrequency", true);
                shootFrequency += "FINISHED";
            }

            string timerInitialTime = "Timer initial time: ";
            if (PlayerData.TimerInitialTimeIndex < Improvements.TimerInitialTimeData.Count - 1)
            {
                timerInitialTime += Improvements.TimerInitialTimeData[PlayerData.TimerInitialTimeIndex + 1].Key + " (" +
                                  Improvements.TimerInitialTimeData[PlayerData.TimerInitialTimeIndex + 1].Value + "$)";
                _finished.Add("timerInitialTime", false);
            }
            else
            {
                _finished.Add("timerInitialTime", true);
                timerInitialTime += "FINISHED";
            }

            string timerExtraTime = "Timer extra time: ";
            if (PlayerData.TimerExtraTimeIndex < Improvements.TimerExtraTimeData.Count - 1)
            {
                timerExtraTime += Improvements.TimerExtraTimeData[PlayerData.TimerExtraTimeIndex + 1].Key + " (" +
                                  Improvements.TimerExtraTimeData[PlayerData.TimerExtraTimeIndex + 1].Value + "$)";
                _finished.Add("timerExtraTime", false);
            }
            else
            {
                _finished.Add("timerExtraTime", true);
                timerExtraTime += "FINISHED";
            }

            string invicibleTime = "Invicible time: ";
            if (PlayerData.InvicibleTimeIndex < Improvements.InvicibleTimeData.Count - 1)
            {
                invicibleTime += Improvements.InvicibleTimeData[PlayerData.InvicibleTimeIndex + 1].Key + " (" +
                                  Improvements.InvicibleTimeData[PlayerData.InvicibleTimeIndex + 1].Value + "$)";
                _finished.Add("invicibleTime", false);
            }
            else
            {
                _finished.Add("invicibleTime", true);
                invicibleTime += "FINISHED";
            }

            string bulletTime = "Bullet time: ";
            if (!PlayerData.BulletTimeEnabled)
            {
                bulletTime += "UNLOCK (" + Improvements.BulletTimePrice + "$)";
                _finished.Add("bulletTime", false);
            }
            else
            {
                _finished.Add("bulletTime", true);
                bulletTime += "UNLOCKED";
            }

            string bulletTimeTimer = "Bullet time timer: ";
            if (PlayerData.BulletTimeTimerIndex < Improvements.BulletTimeTimerData.Count - 1)
            {
                bulletTimeTimer += Improvements.BulletTimeTimerData[PlayerData.BulletTimeTimerIndex + 1].Key + " (" +
                                  Improvements.BulletTimeTimerData[PlayerData.BulletTimeTimerIndex + 1].Value + "$)";
                _finished.Add("bulletTimeTimer", false);
            }
            else
            {
                _finished.Add("bulletTimeTimer", true);
                bulletTimeTimer += "FINISHED";
            }

            string bulletTimeDivisor = "Bullet time divisor: ";
            if (PlayerData.BulletTimeDivisorIndex < Improvements.BulletTimeDivisorData.Count - 1)
            {
                bulletTimeDivisor += "x1/" + (Improvements.BulletTimeDivisorData[PlayerData.BulletTimeDivisorIndex + 1].Key) + " (" +
                                  Improvements.BulletTimeDivisorData[PlayerData.BulletTimeDivisorIndex + 1].Value + "$)";
                _finished.Add("bulletTimeDivisor", false);
            }
            else
            {
                _finished.Add("bulletTimeDivisor", true);
                bulletTimeDivisor += "FINISHED";
            }

            _menuText = new string[]
                {
                    livesNumber,
                    shootType,
                    shootPower,
                    shootFrequency,
                    timerInitialTime,
                    timerExtraTime,
                    invicibleTime,
                    bulletTime,
                    bulletTimeTimer,
                    bulletTimeDivisor
                };
        }

    }
}
