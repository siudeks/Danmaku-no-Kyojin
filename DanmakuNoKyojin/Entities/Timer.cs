using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DanmakuNoKyojin.Framework;

namespace DanmakuNoKyojin.Entities
{
    public class Timer : IGameComponentPart
    {
        private TimeSpan _initTime;
        private TimeSpan _currentTime;
        private bool _isFinished;

        // Fonts
        private SpriteFont _secondsFont;

        public bool IsFinished
        {
            get { return _isFinished; }
        }

        public void Initialize()
        {
            _initTime = Improvements.TimerInitialTimeData[PlayerData.TimerInitialTimeIndex].Key;
            _currentTime = _initTime;
            _isFinished = false;
        }

        public void LoadContent(IContentLoader loader)
        {
            _secondsFont = loader.Load<SpriteFont>("Graphics/Fonts/TimerSeconds");
        }

        public void Update(GameTime gameTime)
        {
            if (!_isFinished)
            {
                if (_currentTime > TimeSpan.Zero)
                    _currentTime -= gameTime.ElapsedGameTime;
                else
                {
                    _currentTime = TimeSpan.Zero;
                    _isFinished = true;
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // base.Draw(gameTime);

            var seconds = Math.Round(_currentTime.TotalSeconds).ToString(CultureInfo.InvariantCulture);
            var milliseconds = string.Format("{0:00}", (_currentTime.Milliseconds / 10)); 

            //_spriteBatch.Begin();

            spriteBatch.DrawString(_secondsFont, seconds, new Vector2(
                Config.Resolution.X / 2f - _secondsFont.MeasureString(seconds).X / 2f,
                -10), Color.White);
            /*
            _spriteBatch.DrawString(_millisecondsFont, milliseconds, new Vector2(
                Config.Resolution.X / 2f + _secondsFont.MeasureString(seconds).X / 2f,
                20), Color.White);
            */
         
            //_spriteBatch.End();
        }

        public void AddTime(TimeSpan extraTime)
        {
            _currentTime = _currentTime.Add(extraTime);
        }
    }
}
