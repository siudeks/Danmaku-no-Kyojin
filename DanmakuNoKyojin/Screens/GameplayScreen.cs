using System.Globalization;
using DanmakuNoKyojin.BulletEngine;
using DanmakuNoKyojin.Controls;
using DanmakuNoKyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using NewConfig = Danmaku.Config;
using DanmakuNoKyojin.Entities.Boss;
using Microsoft.Xna.Framework.Media;
using DanmakuNoKyojin.Camera;

namespace DanmakuNoKyojin.Screens
{
    public class GameplayScreen : BaseGameState
    {
        // Camera
        Viewport defaultView;

        private Texture2D _pixel;

        public Player Player { get; set; }
        private Boss _enemy;

        private int _waveNumber;

        // Audio
        private SoundEffect hit = null;

        // Timer (descending)
        private readonly Timer _timer;

        // Timer for play time
        private TimeSpan _playTime;

        // Background
        private Texture2D _backgroundImage;
        private Rectangle _backgroundMainRectangle;
        private Rectangle _backgroundTopRectangle;

        private Camera2D camera;

        public GameplayScreen(Game game, GameStateManager manager, Camera2D camera)
            : base(game, manager)
        {

            this.camera = camera;

            // Timer
            _timer = new Timer(Game);
        }

        public override void Initialize()
        {
            _backgroundMainRectangle = new Rectangle(0, 0, NewConfig.GameAreaX, NewConfig.GameAreaY);
            _backgroundTopRectangle = new Rectangle(0, -NewConfig.GameAreaY, NewConfig.GameAreaX, NewConfig.GameAreaY);

            _playTime = TimeSpan.Zero;

            _waveNumber = 0;

            _timer.Initialize();

            base.Initialize();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(GameRef.Content.Load<Song>("Audio/Musics/Background"));

            _timer.Initialize();

            defaultView = GraphicsDevice.Viewport;

            // First player
            var player1 = new Player(GameRef, 
                Config.PlayersController[0],
                new Vector2(NewConfig.GameAreaX / 2f, NewConfig.GameAreaY - 150),
                camera);
            player1.Initialize();
            player1.LoadContent(GameRef);
            Player = player1;

            _enemy = new Boss(GameRef, Player);
            _enemy.Initialize();
        }

        protected override void LoadContent()
        {
            if (hit == null)
            {
                hit = GameRef.Content.Load<SoundEffect>(@"Audio/SE/hurt");
            }

            _backgroundImage = GameRef.Content.Load<Texture2D>("Graphics/Pictures/background");
            _pixel = GameRef.Pixel;

            base.LoadContent();
        }

        protected override void UnloadContent()
        {

            Player.Dispose();
            MediaPlayer.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            // Move the background
            if (_backgroundMainRectangle.Y >= Config.Resolution.Y)
                _backgroundMainRectangle.Y = _backgroundTopRectangle.Y - Config.Resolution.Y;
            if (_backgroundTopRectangle.Y >= Config.Resolution.Y)
                _backgroundTopRectangle.Y = _backgroundMainRectangle.Y - Config.Resolution.Y;

            _timer.Update(gameTime);

            _playTime += gameTime.ElapsedGameTime;

            if (InputHandler.PressedCancel())
            {
                UnloadContent();
                StateManager.ChangeState(GameRef.TitleScreen);
            }

            base.Update(gameTime);

            {
                var p = Player;
                if (p.IsAlive)
                {
                    if (p.BulletTime)
                    {
                        var newGameTime = new GameTime(
                            gameTime.TotalGameTime,
                            new TimeSpan((long)(gameTime.ElapsedGameTime.Ticks / Improvements.BulletTimeDivisorData[PlayerData.BulletTimeDivisorIndex].Key))
                        );

                        gameTime = newGameTime;
                    }

                    /*
                    for (int i = 0; i < p.GetBullets().Count; i++)
                    {
                        p.GetBullets()[i].Update(gameTime);

                        if (_enemy.IsAlive && _enemy.Intersects(p.GetBullets()[i]))
                        {
                            if (_enemy.IsReady())
                            {
                                _enemy.TakeDamage(p.GetBullets()[i].Power);
                                hit.Play();
                                p.AddScore(Improvements.ScoreByHitData[PlayerData.ScoreByHitIndex].Key);
                            }

                            p.GetBullets().Remove(p.GetBullets()[i]);
                        }
                        else
                        {
                            if (p.GetBullets()[i].X < 0 || p.GetBullets()[i].X > NewConfig.GameAreaX ||
                                p.GetBullets()[i].Y < 0 || p.GetBullets()[i].Y > NewConfig.GameAreaY)
                            {
                                p.GetBullets().Remove(p.GetBullets()[i]);
                            }
                        }
                    }
                    */

                    foreach (var part in _enemy.Parts)
                    {
                        if (p.Intersects(part))
                        {
                            p.Hit(GameRef.ParticleManager);
                        }
                    }

                    foreach (Mover m in _enemy.MoverManager.movers)
                    {
                        if (p.Intersects(m))
                            p.Hit(GameRef.ParticleManager);
                    }

                    p.Update(gameTime, GameRef, GameRef.SpriteBatch, GameRef.ParticleManager);
                }
            }

            /*
            if (_enemy.IsAlive)
            {
                _enemy.Update(gameTime);
            }
            else
            {
                _waveNumber++;

                _timer.AddTime(Improvements.TimerExtraTimeData[PlayerData.TimerExtraTimeIndex].Key);

                _enemy.DefeatNumber++;
                _enemy.Initialize();
            }
            */

            // GameRef Over
            if (!Player.IsAlive || _timer.IsFinished)
            {
                UnloadContent();

                GameRef.GameOverScreen.Died = !_timer.IsFinished;
                GameRef.GameOverScreen.Time = _playTime;
                GameRef.GameOverScreen.WaveNumber = _waveNumber;
                GameRef.GameOverScreen.Player1Score = Player.Score;

                int totalScore =
                    GameRef.GameOverScreen.Player1Score +
                    GameRef.GameOverScreen.Player2Score +
                    (Improvements.ScoreByEnemyData[PlayerData.ScoreByEnemyIndex].Key * GameRef.GameOverScreen.WaveNumber) +
                    (int)_playTime.TotalSeconds;

                GameRef.GameOverScreen.TotalScore = totalScore;

                PlayerData.Credits += totalScore;

                StateManager.ChangeState(GameRef.GameOverScreen);
            }

            if (Config.Debug && InputHandler.KeyPressed(Keys.C))
                Config.DisplayCollisionBoxes = !Config.DisplayCollisionBoxes;

            camera.Update(InputHandler.MouseState.X, InputHandler.MouseState.Y, Player.Ship.Position, Player.Ship.Rotation);

            _enemy.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin(0, BlendState.Opaque);

            GameRef.SpriteBatch.End();

            GameRef.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Color backgroundColor = new Color(5, 5, 5);
            GraphicsDevice.Clear(backgroundColor);

            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation());

            Color randomColor = Color.White;//new Color(Rand.Next(255), Rand.Next(255), Rand.Next(255));
            GameRef.SpriteBatch.Draw(_backgroundImage, _backgroundMainRectangle, randomColor);

            foreach (var bullet in Player.GetBullets())
            {
                bullet.Draw(gameTime, GameRef.SpriteBatch);
            }

            Player.Draw(gameTime, GameRef.SpriteBatch);

            //if (_enemy.IsAlive)
            //{
               _enemy.Draw(gameTime, camera.GetTransformation());
            //}

            GameRef.SpriteBatch.End();

            base.Draw(gameTime);

            GameRef.SpriteBatch.Begin();

            _timer.Draw(gameTime);

            
            {
                var p = Player;
                if (p.IsAlive)
                {
                    p.DrawString(gameTime, GameRef.SpriteBatch);
                }
            }

            // Text
            if (Config.Debug)
            {
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont,
                                               "Boss bullets: " +
                                               _enemy.MoverManager.movers.Count.ToString(CultureInfo.InvariantCulture),
                                               new Vector2(1, 21), Color.Black);
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont,
                                               "Boss bullets: " +
                                               _enemy.MoverManager.movers.Count.ToString(CultureInfo.InvariantCulture),
                                               new Vector2(0, 20), Color.White);
            }

            // Wave number
            string waveNumber = "Wave #" + _waveNumber.ToString(CultureInfo.InvariantCulture);

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, waveNumber,
                new Vector2(Config.Resolution.X / 2f - ControlManager.SpriteFont.MeasureString(waveNumber).X / 2f + 1, Config.Resolution.Y - 49), Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, waveNumber,
                new Vector2(Config.Resolution.X / 2f - ControlManager.SpriteFont.MeasureString(waveNumber).X / 2f, Config.Resolution.Y - 50), Color.White);

            // Boss current pattern
            /*
            if (Config.Debug)
            {
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _enemy.GetCurrentPatternName(),
                                               new Vector2(
                                                   Config.Resolution.X / 2f -
                                                   ControlManager.SpriteFont.MeasureString(
                                                       _enemy.GetCurrentPatternName()).X / 2,
                                                   Config.Resolution.Y - 25),
                                               Color.Black);
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _enemy.GetCurrentPatternName(),
                                               new Vector2(
                                                   Config.Resolution.X / 2f -
                                                   ControlManager.SpriteFont.MeasureString(
                                                       _enemy.GetCurrentPatternName()).X / 2 + 1,
                                                   Config.Resolution.Y - 26),
                                               Color.White);
            }
            */
            GameRef.SpriteBatch.End();
        }
    }
}
