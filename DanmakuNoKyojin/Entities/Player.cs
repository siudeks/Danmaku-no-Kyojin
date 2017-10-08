using System.Globalization;
using DanmakuNoKyojin.Particles;
using DanmakuNoKyojin.Utils;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using DanmakuNoKyojin.Controls;
using Microsoft.Xna.Framework.Input;
using DanmakuNoKyojin.Collisions;

using DanmakuNoKyojin.Camera;
using Akka.Actor;

using ShipActor = Danmaku.ShipActor;

namespace DanmakuNoKyojin.Entities
{
    public sealed class Player : BulletLauncherEntity, IDisposable
    {
        private ShipView ship;

        private static Random random = new Random();

        private Config.Controller _controller;

        private Viewport _viewport;

        private Texture2D _bulletSprite;
        private Texture2D _hitboxSprite;

        private float _hitboxRadius;

        private Vector2 _originPosition;

        // Bullet Time
        public bool BulletTime { get; set; }
        private Texture2D _bulletTimeBarLeft;
        private Texture2D _bulletTimeBarContent;
        private Texture2D _bulletTimeBarRight;
        private TimeSpan _bulletTimeTimer;
        private bool _bulletTimeReloading;

        private int _lives;
        public bool IsInvincible { get; set; }
        private TimeSpan _invincibleTime;
        private TimeSpan _timeBeforeRespawn;

        // Shield
        private Texture2D _shieldSprite;
        private Vector2 _shieldOrigin;
        private CollisionCircle _shieldCollisionCircle;

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

        public Player(GameRunner gameRef, Viewport viewport, Config.Controller controller, Vector2 position)
            : base(gameRef)
        {
            _viewport = viewport;
            _controller = controller;
            _originPosition = position;
            Position = _originPosition;
            Origin = Vector2.Zero;
            _random = new Random();
            _cameraPosition = new Vector2(_viewport.Width / 2f, _viewport.Height / 2f);
            _focusMode = false;
            _timeBeforeRespawn = TimeSpan.Zero;
        }

        public override void Initialize()
        {
            Rotation = 0f;
            //_distance = Vector2.Zero;

            _lives = 10;//Improvements.LivesNumberData[PlayerData.LivesNumberIndex].Key;

            IsInvincible = false;
            _invincibleTime = Config.PlayerInvicibleTime;

            BulletTime = false;
            BulletFrequence = new TimeSpan(0);
            IsAlive = true;

            _score = 0;
            _bulletTimeTimer = Config.DefaultBulletTimeTimer;
            _hitboxRadius = (float)Math.PI * 1.5f * 2;
            _camera = new Camera2D(_viewport, 1f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Sprite = GameRef.Content.Load<Texture2D>("Graphics/Entities/player");
            _bulletSprite = GameRef.Content.Load<Texture2D>("Graphics/Entities/player_bullet");
            _hitboxSprite = GameRef.Content.Load<Texture2D>("Graphics/Pictures/player_hitbox");
            CollisionBoxes.Add(new CollisionCircle(this, new Vector2(Sprite.Height / 6f, Sprite.Height / 6f), _hitboxRadius / 2f));

            _shieldSprite = GameRef.Content.Load<Texture2D>("Graphics/Entities/shield");
            _shieldOrigin = new Vector2(_shieldSprite.Width / 2f, _shieldSprite.Height / 2f);
            _shieldCollisionCircle = new CollisionCircle(this, Vector2.Zero, _shieldSprite.Width / 2f);

            _lifeIcon = GameRef.Content.Load<Texture2D>("Graphics/Pictures/life_icon");

            _bulletTimeBarLeft = GameRef.Content.Load<Texture2D>("Graphics/Pictures/gauge_left");
            _bulletTimeBarContent = GameRef.Content.Load<Texture2D>("Graphics/Pictures/gauge_middle");
            _bulletTimeBarRight = GameRef.Content.Load<Texture2D>("Graphics/Pictures/gauge_right");

            if (_shootSound == null)
                _shootSound = GameRef.Content.Load<SoundEffect>(@"Audio/SE/hit");
            if (_deadSound == null)
                _deadSound = GameRef.Content.Load<SoundEffect>(@"Audio/SE/dead");

            ship = new ShipView(Program.system, Sprite);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_lives <= 0)
                IsAlive = false;

            if (IsInvincible)
            {
                if (_timeBeforeRespawn.TotalMilliseconds > 0)
                {
                    _timeBeforeRespawn -= gameTime.ElapsedGameTime;

                    if (_timeBeforeRespawn.TotalMilliseconds <= 0)
                    {
                        Position = _originPosition;
                    }
                }
                else
                {
                    _invincibleTime -= gameTime.ElapsedGameTime;

                    if (_invincibleTime.TotalMilliseconds <= 0)
                    {
                        _invincibleTime = Config.PlayerInvicibleTime;
                        IsInvincible = false;
                        CollisionBoxes.Remove(_shieldCollisionCircle);
                    }
                }
            }
            //else
            {
                var inputState = ReadInput(_controller, _viewport);

                Rotation = inputState.Rotation;
                ship.ChangeDirection(inputState.Direction);  // .Tell(new ShipActor.ChangeDirection(inputState.Direction.X, inputState.Direction.Y));

                BulletTime = (PlayerData.BulletTimeEnabled && (!_bulletTimeReloading && inputState.BulletTime)) ? true : false;

                if (inputState.Fire)
                {
                    Fire(gameTime);
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

                ship.Update(gameTime);
            }

            // Update camera position
            _cameraPosition.X = MathHelper.Lerp(
                _cameraPosition.X,
                Position.X - Config.CameraDistanceFromPlayer.X * (float)Math.Cos(Rotation + MathHelper.PiOver2),
                Config.CameraMotionInterpolationAmount
            );

            _cameraPosition.Y = MathHelper.Lerp(
                _cameraPosition.Y,
                Position.Y - Config.CameraDistanceFromPlayer.Y * (float)Math.Sin(Rotation + MathHelper.PiOver2),
                Config.CameraMotionInterpolationAmount
            );

            _camera.Update(_cameraPosition);

            if (!Config.DisableCameraZoom)
            {
                // Update camera zoom according to mouse distance from player
                var mouseWorldPosition = new Vector2(
                    _cameraPosition.X - GameRef.Graphics.GraphicsDevice.Viewport.Width / 2f + InputHandler.MouseState.X,
                    _cameraPosition.Y - GameRef.Graphics.GraphicsDevice.Viewport.Height / 2f + InputHandler.MouseState.Y
                    );

                var mouseDistanceFromPlayer =
                    (float)
                        Math.Sqrt(Math.Pow(Position.X - mouseWorldPosition.X, 2) +
                                  Math.Pow(Position.Y - mouseWorldPosition.Y, 2));

                var cameraZoom = GameRef.Graphics.GraphicsDevice.Viewport.Width / mouseDistanceFromPlayer;

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

        private static InputData ReadInputFromKeyboard(Viewport _viewport)
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

            var distanceX = (_viewport.Width / 2f) - InputHandler.MouseState.X;
            var distanceY = (_viewport.Height / 2f) - InputHandler.MouseState.Y;
            var rotation = (float)Math.Atan2(distanceY, distanceX) - MathHelper.PiOver2;

            return new InputData(bulletTime, fire, direction, rotation);
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

            return new InputData(bulletTime, fire, direction, rotation);
        }

        private static InputData ReadInput(Config.Controller controller, Viewport viewport)
        {
            if (controller == Config.Controller.Keyboard)
                return ReadInputFromKeyboard(viewport);

            if (controller == Config.Controller.GamePad)
                return ReadInputFromPad();

            throw new ArgumentException(nameof(controller));
        }

        public override void Draw(GameTime gameTime)
        {
            // for testing purposes let ask synchronously about ship position
            // later we need to improve it in async way.
            var status = ship.GetStatus();

            Position = new Vector2(status.PositionX, status.PositionY);

            if (_timeBeforeRespawn.TotalMilliseconds <= 0)
            {
                GameRef.SpriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, Origin, 1f, SpriteEffects.None, 0f);

                if (IsInvincible)
                    GameRef.SpriteBatch.Draw(_shieldSprite, Position, null, Color.White, 0f, new Vector2(_shieldSprite.Width / 2f, _shieldSprite.Height / 2f), 1f, SpriteEffects.None, 0f);
            }

            base.Draw(gameTime);
        }

        public void DrawString(GameTime gameTime)
        {
            // Text
            string lives = "P1";
            string score = string.Format("{0:000000000000}", _score);


            // Lives
            int hudY = 40;

            if (PlayerData.BulletTimeEnabled)
                hudY = 80;

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, lives, new Vector2(1, Config.Resolution.Y - hudY + 1), Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, lives, new Vector2(0, Config.Resolution.Y - hudY), Color.White);

            for (int i = 0; i < _lives; i++)
            {
                GameRef.SpriteBatch.Draw(_lifeIcon, new Vector2(
                    ControlManager.SpriteFont.MeasureString(lives).X + i * _lifeIcon.Width + 10, Config.Resolution.Y - (hudY - 7)), Color.White);
            }

            // Bullet time bar
            if (PlayerData.BulletTimeEnabled)
            {
                int bulletTimeBarWidth =
                    (int)
                    (100 * (float)(_bulletTimeTimer.TotalMilliseconds / Config.DefaultBulletTimeTimer.TotalMilliseconds));

                // Text
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont,
                                            bulletTimeBarWidth.ToString(CultureInfo.InvariantCulture),
                                            new Vector2(1, Config.Resolution.Y - 39), Color.Black);
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont,
                                            bulletTimeBarWidth.ToString(CultureInfo.InvariantCulture),
                                            new Vector2(0, Config.Resolution.Y - 40), Color.White);

                // Bar
                GameRef.SpriteBatch.Draw(_bulletTimeBarLeft,
                                      new Rectangle(0, Config.Resolution.Y - 50, _bulletTimeBarLeft.Width, _bulletTimeBarLeft.Height),
                                      Color.White);
                GameRef.SpriteBatch.Draw(_bulletTimeBarContent,
                                      new Rectangle(_bulletTimeBarLeft.Width, Config.Resolution.Y - 50, bulletTimeBarWidth,
                                                    _bulletTimeBarContent.Height), Color.White);
                GameRef.SpriteBatch.Draw(_bulletTimeBarRight,
                                      new Rectangle(_bulletTimeBarLeft.Width + bulletTimeBarWidth, Config.Resolution.Y - 50,
                                                    _bulletTimeBarRight.Width, _bulletTimeBarRight.Height),
                                      Color.White);
            }

            // Score
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, score, new Vector2(1, Config.Resolution.Y - 20), Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, score, new Vector2(0, Config.Resolution.Y - 21), Color.White);

        }

        private void Fire(GameTime gameTime)
        {
            if (BulletFrequence.TotalMilliseconds > 0)
                BulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                BulletFrequence = TimeSpan.FromTicks((long)(Improvements.ShootFrequencyData[Improvements.ShootFrequencyData.Count - 3].Key * Config.PlayerShootFrequency.Ticks));

                var direction = new Vector2((float)Math.Sin(Rotation), (float)Math.Cos(Rotation) * -1);

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

                    var bullet = new Bullet(GameRef, _bulletSprite, Position + offset, direction, Config.PlayerBulletVelocity)
                    {
                        WaveMode = false
                    };
                    AddBullet(bullet);
                }

                // Front sides 1/2 diagonal
                if (PlayerData.ShootTypeIndex > 0)
                {
                    Vector2 directionLeft = direction;
                    Vector2 positionLeft = new Vector2(Position.X - 25f * (float)Math.Cos(Rotation), Position.Y - 25f * (float)Math.Sin(Rotation));
                    Vector2 directionRight = direction;
                    Vector2 positionRight = new Vector2(Position.X + 25f * (float)Math.Cos(Rotation), Position.Y + 25f * (float)Math.Sin(Rotation));

                    directionLeft = new Vector2((float)Math.Sin(Rotation - Math.PI / 4), (float)Math.Cos(Rotation - Math.PI / 4) * -1);
                    directionRight = new Vector2((float)Math.Sin(Rotation + Math.PI / 4), (float)Math.Cos(Rotation + Math.PI / 4) * -1);

                    var bulletLeft = new Bullet(GameRef, _bulletSprite, positionLeft, directionLeft, Config.PlayerBulletVelocity);

                    var bulletRight = new Bullet(GameRef, _bulletSprite, positionRight, directionRight, Config.PlayerBulletVelocity);

                    AddBullet(bulletLeft);
                    AddBullet(bulletRight);
                }

                // Front sides 1/4 diagonal
                if (PlayerData.ShootTypeIndex >= 3)
                {
                    Vector2 directionLeft = direction;
                    Vector2 positionLeft = new Vector2(Position.X - 10f * (float)Math.Cos(Rotation), Position.Y - 10f * (float)Math.Sin(Rotation));
                    Vector2 directionRight = direction;
                    Vector2 positionRight = new Vector2(Position.X + 10f * (float)Math.Cos(Rotation), Position.Y + 10f * (float)Math.Sin(Rotation));

                    directionLeft = new Vector2((float)Math.Sin(Rotation - Math.PI / 8), (float)Math.Cos(Rotation - Math.PI / 8) * -1);
                    directionRight = new Vector2((float)Math.Sin(Rotation + Math.PI / 8), (float)Math.Cos(Rotation + Math.PI / 8) * -1);

                    var bulletLeft = new Bullet(GameRef, _bulletSprite, positionLeft, directionLeft, Config.PlayerBulletVelocity);
                    bulletLeft.Power = 0.5f;

                    var bulletRight = new Bullet(GameRef, _bulletSprite, positionRight, directionRight, Config.PlayerBulletVelocity);
                    bulletRight.Power = 0.5f;

                    AddBullet(bulletLeft);
                    AddBullet(bulletRight);
                }

                // Behind
                if (PlayerData.ShootTypeIndex >= 2)
                {
                    var directionBehind = new Vector2((float)Math.Sin(Rotation) * -1, (float)Math.Cos(Rotation));

                    var bullet = new Bullet(GameRef, _bulletSprite, Position, directionBehind, Config.PlayerBulletVelocity);
                    bullet.Power = Improvements.ShootPowerData[PlayerData.ShootPowerIndex].Key;
                    bullet.WaveMode = false;

                    AddBullet(bullet);
                }

                _shootSound.Play();

                //FireParticles(gameTime);
            }
        }

        public void Hit()
        {
            if (!IsInvincible)
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

                    GameRef.ParticleManager.CreateParticle(GameRef.LineParticle, Position, color, 190, 1.5f, state);
                }

                _timeBeforeRespawn = Config.PlayerTimeBeforeRespawn;
                IsInvincible = true;
                CollisionBoxes.Add(_shieldCollisionCircle);
            }
        }

        public void AddScore(int value)
        {
            _score += value;
        }

        private void FireParticles(GameTime gameTime)
        {
            double t = gameTime.TotalGameTime.TotalSeconds;

            var direction = new Vector2((float)Math.Sin(Rotation), (float)Math.Cos(Rotation) * -1);
            float aimAngle = direction.ToAngle();
            Quaternion rot = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

            // The primary velocity of the particles is 3 pixels/frame in the direction opposite to which the ship is travelling.
            Vector2 baseVel = direction.ScaleTo(2);

            // Calculate the sideways velocity for the two side streams. The direction is perpendicular to the ship's velocity and the
            // magnitude varies sinusoidally.
            var perpVel = new Vector2(baseVel.Y, -baseVel.X) * (0.6f * (float)Math.Sin(t * 10));
            var sideColor = new Color(200, 38, 9);    // deep red
            var midColor = new Color(255, 187, 30);   // orange-yellow
            var pos = Position;   // position of the ship's exhaust pipe.
            const float alpha = 0.7f;

            // middle particle stream
            Vector2 velMid = baseVel + random.NextVector2(0, 1);

            GameRef.ParticleManager.CreateParticle(GameRef.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(velMid, ParticleType.Enemy));
            GameRef.ParticleManager.CreateParticle(GameRef.Glow, pos, midColor * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(velMid, ParticleType.Enemy));

            // side particle streams
            Vector2 vel1 = baseVel + perpVel + random.NextVector2(0, 0.3f);
            Vector2 vel2 = baseVel - perpVel + random.NextVector2(0, 0.3f);

            GameRef.ParticleManager.CreateParticle(GameRef.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(vel1, ParticleType.Enemy));
            GameRef.ParticleManager.CreateParticle(GameRef.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(vel2, ParticleType.Enemy));

            GameRef.ParticleManager.CreateParticle(
                GameRef.Glow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(vel1, ParticleType.Enemy)
            );
            GameRef.ParticleManager.CreateParticle(
                GameRef.Glow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(vel2, ParticleType.Enemy)
            );
        }

        public void Dispose()
        {
            ship.Dispose();
        }
    }

    struct InputData
    {
        public bool BulletTime;
        public bool Fire;
        public Vector2 Direction;
        public float Rotation;

        public InputData(bool bulletTime, bool fire, Vector2 direction, float rotation)
        {
            BulletTime = bulletTime;
            Fire = fire;
            Direction = direction;
            Rotation = rotation;
        }
    }

}
