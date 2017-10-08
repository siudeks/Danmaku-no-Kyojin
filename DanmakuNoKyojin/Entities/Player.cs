using System.Globalization;
using DanmakuNoKyojin.Particles;
using DanmakuNoKyojin.Utils;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using DanmakuNoKyojin.Controls;
using Microsoft.Xna.Framework.Input;

using DanmakuNoKyojin.Camera;

using System.Collections.Generic;
using DanmakuNoKyojin.Framework;

namespace DanmakuNoKyojin.Entities
{
    public sealed class Player : IDisposable
    {
        public bool IsAlive { get; set; }

        public ShipView Ship { get; private set; }

        private static Random random = new Random();

        private Config.Controller _controller;

        private Viewport _viewport;

        private Texture2D _bulletSprite;
        private Texture2D _hitboxSprite;

        private Vector2 _originPosition;

        // Bullet Time
        public bool BulletTime { get; set; }
        private Texture2D _bulletTimeBarLeft;
        private Texture2D _bulletTimeBarContent;
        private Texture2D _bulletTimeBarRight;
        private TimeSpan _bulletTimeTimer;
        private bool _bulletTimeReloading;

        private int _lives;
        private TimeSpan _invincibleTime;
        private TimeSpan _timeBeforeRespawn;

        // HUD
        private int _score;
        private Texture2D _lifeIcon;

        // Audio
        private SoundEffect _shootSound = null;
        private SoundEffect _deadSound = null;

        // Camera
        private Camera2D _camera;
        private Vector2 _cameraPosition;
        private bool _focusMode;

        // Random
        private static Random _random;

        public int Score
        {
            get { return _score; }
        }

        public Camera2D Camera
        {
            get { return _camera; }
        }

        public Player(Viewport viewport, Config.Controller controller, Vector2 position)
        {
            _viewport = viewport;
            _controller = controller;
            _originPosition = position;
            _random = new Random();
            _cameraPosition = new Vector2(_viewport.Width / 2f, _viewport.Height / 2f);
            _focusMode = false;
            _timeBeforeRespawn = TimeSpan.Zero;
        }

        private readonly List<BulletView> Bullets = new List<BulletView>();
        private TimeSpan BulletFrequence;

        public List<BulletView> GetBullets()
        {
            return Bullets;
        }

        public void Initialize()
        {
            _lives = 10;//Improvements.LivesNumberData[PlayerData.LivesNumberIndex].Key;

            _invincibleTime = Config.PlayerInvicibleTime;

            BulletTime = false;
            BulletFrequence = new TimeSpan(0);
            IsAlive = true;

            _score = 0;
            _bulletTimeTimer = Config.DefaultBulletTimeTimer;
            _camera = new Camera2D(_viewport, 1f);
        }

        public void LoadContent(IContentLoader contentLoader)
        {
            _bulletSprite = contentLoader.Load<Texture2D>("Graphics/Entities/player_bullet");
            _hitboxSprite = contentLoader.Load<Texture2D>("Graphics/Pictures/player_hitbox");

            _lifeIcon = contentLoader.Load<Texture2D>("Graphics/Pictures/life_icon");

            _bulletTimeBarLeft = contentLoader.Load<Texture2D>("Graphics/Pictures/gauge_left");
            _bulletTimeBarContent = contentLoader.Load<Texture2D>("Graphics/Pictures/gauge_middle");
            _bulletTimeBarRight = contentLoader.Load<Texture2D>("Graphics/Pictures/gauge_right");

            _shootSound = contentLoader.Load<SoundEffect>(@"Audio/SE/hit");
            _deadSound = contentLoader.Load<SoundEffect>(@"Audio/SE/dead");

            Ship = new ShipView(Program.system);
            Ship.LoadContent(contentLoader);
        }

        public void Update(GameTime gameTime, IViewportProvider viewport, SpriteBatch spriteBatch, ParticleManager<ParticleState> particleManager)
        {
            if (_lives <= 0)
                IsAlive = false;

            var inputState = ReadInput(_controller, _viewport, spriteBatch);

            Ship.Command(gameTime.ElapsedGameTime, inputState.Forward, inputState.Rotation);  // .Tell(new ShipActor.ChangeDirection(inputState.Direction.X, inputState.Direction.Y));
            // Ship.ChangeDirection(inputState.Direction, inputState.Rotation);  // .Tell(new ShipActor.ChangeDirection(inputState.Direction.X, inputState.Direction.Y));

            BulletTime = (PlayerData.BulletTimeEnabled && (!_bulletTimeReloading && inputState.BulletTime)) ? true : false;

            if (inputState.Fire)
            {
                Fire(gameTime, particleManager);
            }

            if (inputState.Fire)
            {
                _focusMode = true;
            }
            else if (_focusMode)
            {
                _focusMode = false;
            }

            if (BulletTime)
            {
                _bulletTimeTimer -= gameTime.ElapsedGameTime;

                if (_bulletTimeTimer <= TimeSpan.Zero)
                {
                    _bulletTimeReloading = true;
                    _bulletTimeTimer = TimeSpan.Zero;
                }
            }

            if (_bulletTimeReloading)
            {
                _bulletTimeTimer += gameTime.ElapsedGameTime;

                if (_bulletTimeTimer >= Config.DefaultBulletTimeTimer)
                {
                    _bulletTimeReloading = false;
                    _bulletTimeTimer = Config.DefaultBulletTimeTimer;
                }
            }

            Ship.Update(gameTime);

            // Update camera position
            _cameraPosition.X = MathHelper.Lerp(
                _cameraPosition.X,
                Ship.Position.X - Config.CameraDistanceFromPlayer.X * (float)Math.Cos(Ship.Rotation + MathHelper.PiOver2),
                Config.CameraMotionInterpolationAmount
            );

            _cameraPosition.Y = MathHelper.Lerp(
                _cameraPosition.Y,
                Ship.Position.Y - Config.CameraDistanceFromPlayer.Y * (float)Math.Sin(Ship.Rotation + MathHelper.PiOver2),
                Config.CameraMotionInterpolationAmount
            );

            _camera.Update(_cameraPosition);

            if (!Config.DisableCameraZoom)
            {
                // Update camera zoom according to mouse distance from player
                var mouseWorldPosition = new Vector2(
                    _cameraPosition.X - viewport.Width / 2f + InputHandler.MouseState.X,
                    _cameraPosition.Y - viewport.Height / 2f + InputHandler.MouseState.Y
                    );

                var mouseDistanceFromPlayer =
                    (float)
                        Math.Sqrt(Math.Pow(Ship.Position.X - mouseWorldPosition.X, 2) +
                                  Math.Pow(Ship.Position.Y - mouseWorldPosition.Y, 2));

                var cameraZoom = viewport.Width / mouseDistanceFromPlayer;

                if (_focusMode)
                    cameraZoom = 1f;
                else
                    cameraZoom = cameraZoom > Config.CameraZoomLimit
                        ? 1f
                        : MathHelper.Clamp(cameraZoom / Config.CameraZoomLimit, Config.CameraZoomMin, Config.CameraZoomMax);

                _camera.Zoom = MathHelper.Lerp(_camera.Zoom, cameraZoom, Config.CameraZoomInterpolationAmount);
            }

            foreach (var bullet in Bullets)
            {
                bullet.Update(gameTime);
            }
        }

        private static InputData ReadInputFromKeyboard(Viewport _viewport, SpriteBatch spriteBatch)
        {
            var bulletTime = InputHandler.MouseState.RightButton == ButtonState.Pressed;

            var fire = InputHandler.MouseState.LeftButton == ButtonState.Pressed;

            var direction = Vector2.Zero;
            if (InputHandler.KeyDown(Config.PlayerKeyboardInputs["Up"]))
                direction.Y = -1;
            if (InputHandler.KeyDown(Config.PlayerKeyboardInputs["Right"]))
                direction.X = 1;
            if (InputHandler.KeyDown(Config.PlayerKeyboardInputs["Down"]))
                direction.Y = 1;
            if (InputHandler.KeyDown(Config.PlayerKeyboardInputs["Left"]))
                direction.X = -1;
            var forward = InputHandler.KeyDown(Config.PlayerKeyboardInputs["Forward"]);

            var distanceX = (_viewport.Width / 2f) - InputHandler.MouseState.X;
            var distanceY = (_viewport.Height / 2f) - InputHandler.MouseState.Y;
            var rotation = (float)Math.Atan2(distanceY, distanceX) - MathHelper.PiOver2;

            return new InputData(bulletTime, fire, direction, rotation, forward);
        }

        private static InputData ReadInputFromPad()
        {
            var bulletTime = InputHandler.ButtonDown(Config.PlayerGamepadInput[1], PlayerIndex.One);

            var fire = InputHandler.GamePadStates[0].ThumbSticks.Right.Length() > 0;

            var direction = Vector2.Zero;
            direction.X = InputHandler.GamePadStates[0].ThumbSticks.Left.X;
            direction.Y = (-1) * InputHandler.GamePadStates[0].ThumbSticks.Left.Y;

            var rotation = (float)
                Math.Atan2(InputHandler.GamePadStates[0].ThumbSticks.Right.Y * (-1),
               InputHandler.GamePadStates[0].ThumbSticks.Right.X) + MathHelper.PiOver2;

            return new InputData(bulletTime, fire, direction, rotation, false);
        }

        private static InputData ReadInput(Config.Controller controller, Viewport viewport, SpriteBatch spriteBatch)
        {
            if (controller == Config.Controller.Keyboard)
                return ReadInputFromKeyboard(viewport, spriteBatch);

            if (controller == Config.Controller.GamePad)
                return ReadInputFromPad();

            throw new ArgumentException(nameof(controller));
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Ship.Draw(gameTime, spriteBatch);
        }

        public void DrawString(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Text
            string lives = "P1";
            string score = string.Format("{0:000000000000}", _score);


            // Lives
            int hudY = 40;

            if (PlayerData.BulletTimeEnabled)
                hudY = 80;

            spriteBatch.DrawString(ControlManager.SpriteFont, lives, new Vector2(1, Config.Resolution.Y - hudY + 1), Color.Black);
            spriteBatch.DrawString(ControlManager.SpriteFont, lives, new Vector2(0, Config.Resolution.Y - hudY), Color.White);

            for (int i = 0; i < _lives; i++)
            {
                spriteBatch.Draw(_lifeIcon, new Vector2(
                    ControlManager.SpriteFont.MeasureString(lives).X + i * _lifeIcon.Width + 10, Config.Resolution.Y - (hudY - 7)), Color.White);
            }

            // Bullet time bar
            if (PlayerData.BulletTimeEnabled)
            {
                int bulletTimeBarWidth =
                    (int)
                    (100 * (float)(_bulletTimeTimer.TotalMilliseconds / Config.DefaultBulletTimeTimer.TotalMilliseconds));

                // Text
                spriteBatch.DrawString(ControlManager.SpriteFont,
                                            bulletTimeBarWidth.ToString(CultureInfo.InvariantCulture),
                                            new Vector2(1, Config.Resolution.Y - 39), Color.Black);
                spriteBatch.DrawString(ControlManager.SpriteFont,
                                            bulletTimeBarWidth.ToString(CultureInfo.InvariantCulture),
                                            new Vector2(0, Config.Resolution.Y - 40), Color.White);

                // Bar
                spriteBatch.Draw(_bulletTimeBarLeft,
                                      new Rectangle(0, Config.Resolution.Y - 50, _bulletTimeBarLeft.Width, _bulletTimeBarLeft.Height),
                                      Color.White);
                spriteBatch.Draw(_bulletTimeBarContent,
                                      new Rectangle(_bulletTimeBarLeft.Width, Config.Resolution.Y - 50, bulletTimeBarWidth,
                                                    _bulletTimeBarContent.Height), Color.White);
                spriteBatch.Draw(_bulletTimeBarRight,
                                      new Rectangle(_bulletTimeBarLeft.Width + bulletTimeBarWidth, Config.Resolution.Y - 50,
                                                    _bulletTimeBarRight.Width, _bulletTimeBarRight.Height),
                                      Color.White);
            }

            // Score
            spriteBatch.DrawString(ControlManager.SpriteFont, score, new Vector2(1, Config.Resolution.Y - 20), Color.Black);
            spriteBatch.DrawString(ControlManager.SpriteFont, score, new Vector2(0, Config.Resolution.Y - 21), Color.White);

        }

        private void Fire(GameTime gameTime, ParticleManager<ParticleState> particleManager)
        {
            if (BulletFrequence.TotalMilliseconds > 0)
                BulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                BulletFrequence = TimeSpan.FromTicks((long)(Improvements.ShootFrequencyData[Improvements.ShootFrequencyData.Count - 3].Key * Config.PlayerShootFrequency.Ticks));

                var direction = new Vector2((float)Math.Sin(Ship.Rotation), (float)Math.Cos(Ship.Rotation) * -1);

                // Straight
                if (PlayerData.ShootTypeIndex != 1)
                {
                    // Add randomness to bullet direction
                    float aimAngle = direction.ToAngle();
                    Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);
                    float spreadAmount = 0.25f;

                    if (_focusMode)
                        spreadAmount /= 3f;

                    float randomSpread = random.NextFloat(-spreadAmount, spreadAmount);

                    direction = MathUtil.FromPolar(aimAngle + randomSpread, 11f);
                    direction.Normalize();

                    Vector2 offset = Vector2.Transform(new Vector2(0, 0), aimQuat);

                    var bullet = new BulletView(_bulletSprite, Ship.Position + offset, direction, Config.PlayerBulletVelocity)
                    {
                        WaveMode = false
                    };
                    Bullets.Add(bullet);
                }

                // Front sides 1/2 diagonal
                if (PlayerData.ShootTypeIndex > 0)
                {
                    Vector2 directionLeft = direction;
                    Vector2 positionLeft = new Vector2(Ship.Position.X - 25f * (float)Math.Cos(Ship.Rotation), Ship.Position.Y - 25f * (float)Math.Sin(Ship.Rotation));
                    Vector2 directionRight = direction;
                    Vector2 positionRight = new Vector2(Ship.Position.X + 25f * (float)Math.Cos(Ship.Rotation), Ship.Position.Y + 25f * (float)Math.Sin(Ship.Rotation));

                    directionLeft = new Vector2((float)Math.Sin(Ship.Rotation - Math.PI / 4), (float)Math.Cos(Ship.Rotation - Math.PI / 4) * -1);
                    directionRight = new Vector2((float)Math.Sin(Ship.Rotation + Math.PI / 4), (float)Math.Cos(Ship.Rotation + Math.PI / 4) * -1);

                    var bulletLeft = new BulletView(_bulletSprite, positionLeft, directionLeft, Config.PlayerBulletVelocity);

                    var bulletRight = new BulletView(_bulletSprite, positionRight, directionRight, Config.PlayerBulletVelocity);

                    Bullets.Add(bulletLeft);
                    Bullets.Add(bulletRight);
                }

                // Front sides 1/4 diagonal
                if (PlayerData.ShootTypeIndex >= 3)
                {
                    Vector2 directionLeft = direction;
                    Vector2 positionLeft = new Vector2(Ship.Position.X - 10f * (float)Math.Cos(Ship.Rotation), Ship.Position.Y - 10f * (float)Math.Sin(Ship.Rotation));
                    Vector2 directionRight = direction;
                    Vector2 positionRight = new Vector2(Ship.Position.X + 10f * (float)Math.Cos(Ship.Rotation), Ship.Position.Y + 10f * (float)Math.Sin(Ship.Rotation));

                    directionLeft = new Vector2((float)Math.Sin(Ship.Rotation - Math.PI / 8), (float)Math.Cos(Ship.Rotation - Math.PI / 8) * -1);
                    directionRight = new Vector2((float)Math.Sin(Ship.Rotation + Math.PI / 8), (float)Math.Cos(Ship.Rotation + Math.PI / 8) * -1);

                    var bulletLeft = new BulletView(_bulletSprite, positionLeft, directionLeft, Config.PlayerBulletVelocity);
                    bulletLeft.Power = 0.5f;

                    var bulletRight = new BulletView(_bulletSprite, positionRight, directionRight, Config.PlayerBulletVelocity);
                    bulletRight.Power = 0.5f;

                    Bullets.Add(bulletLeft);
                    Bullets.Add(bulletRight);
                }

                // Behind
                if (PlayerData.ShootTypeIndex >= 2)
                {
                    var directionBehind = new Vector2((float)Math.Sin(Ship.Rotation) * -1, (float)Math.Cos(Ship.Rotation));

                    var bullet = new BulletView(_bulletSprite, Ship.Position, directionBehind, Config.PlayerBulletVelocity);
                    bullet.Power = Improvements.ShootPowerData[PlayerData.ShootPowerIndex].Key;
                    bullet.WaveMode = false;

                    Bullets.Add(bullet);
                }

                _shootSound.Play();

                FireParticles(gameTime, particleManager);
            }
        }

        public void Hit(ParticleManager<ParticleState> particleManager)
        {
            _lives--;
            _deadSound.Play();

            var yellow = new Color(0.8f, 0.8f, 0.4f);

            for (int i = 0; i < 1200; i++)
            {
                float speed = 18f * (1f - 1 / random.NextFloat(1f, 10f));
                Color color = Color.Lerp(Color.White, yellow, random.NextFloat(0, 1));
                var state = new ParticleState()
                {
                    Velocity = random.NextVector2(speed, speed),
                    Type = ParticleType.None,
                    LengthMultiplier = 1
                };

                particleManager.CreateLineParticle(Ship.Position, color, 190, 1.5f, state);
            }

            _timeBeforeRespawn = Config.PlayerTimeBeforeRespawn;
            Ship.EnableShield();
        }

        public void AddScore(int value)
        {
            _score += value;
        }

        private void FireParticles(GameTime gameTime, ParticleManager<ParticleState> particleManager)
        {
            double t = gameTime.TotalGameTime.TotalSeconds;

            var direction = new Vector2((float)Math.Sin(Ship.Rotation), (float)Math.Cos(Ship.Rotation) * -1);
            float aimAngle = direction.ToAngle();
            Quaternion rot = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

            // The primary velocity of the particles is 3 pixels/frame in the direction opposite to which the ship is travelling.
            Vector2 baseVel = direction.ScaleTo(2);

            // Calculate the sideways velocity for the two side streams. The direction is perpendicular to the ship's velocity and the
            // magnitude varies sinusoidally.
            var perpVel = new Vector2(baseVel.Y, -baseVel.X) * (0.6f * (float)Math.Sin(t * 10));
            var sideColor = new Color(200, 38, 9);    // deep red
            var midColor = new Color(255, 187, 30);   // orange-yellow
            var pos = Ship.Position;   // position of the ship's exhaust pipe.
            const float alpha = 0.7f;

            // middle particle stream
            Vector2 velMid = baseVel + random.NextVector2(0, 1);

            particleManager.CreateLineParticle(pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(velMid, ParticleType.Enemy));
            particleManager.CreateGlowParticle(pos, midColor * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(velMid, ParticleType.Enemy));

            // side particle streams
            Vector2 vel1 = baseVel + perpVel + random.NextVector2(0, 0.3f);
            Vector2 vel2 = baseVel - perpVel + random.NextVector2(0, 0.3f);

            particleManager.CreateLineParticle(pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(vel1, ParticleType.Enemy));
            particleManager.CreateLineParticle(pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(vel2, ParticleType.Enemy));

            particleManager.CreateGlowParticle(
                pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(vel1, ParticleType.Enemy)
            );
            particleManager.CreateGlowParticle(
                pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(vel2, ParticleType.Enemy)
            );
        }

        public bool Intersects(Entity entity)
        {
            return Ship.Intersects(entity.CollisionBoxes);
        }

        public void Dispose()
        {
            Ship.Dispose();
        }
    }

    struct InputData
    {
        public bool BulletTime;
        public bool Fire;
        public Vector2 Direction;
        public float Rotation;
        public bool Forward;

        public InputData(bool bulletTime, bool fire, Vector2 direction, float rotation, bool forward)
        {
            BulletTime = bulletTime;
            Fire = fire;
            Direction = direction;
            Rotation = rotation;
            Forward = forward;
        }
    }

}
