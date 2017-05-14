﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Danmaku_no_Kyojin
{
    public static class Config
    {
        public static bool Debug = true;
        public static bool Cheat = false;

        public enum Controller
        {
            Keyboard,
            GamePad
        };

        // Debug
        public static bool DisplayCollisionBoxes = false;
        public const bool FpsCapping = true;

        // Screen
        public static bool FullScreen = false;
        public static readonly Point Resolution = new Point(1280, 720);

        // Bullet Time
        public const float DesiredTimeModifier = 2f;

        // Player
        public static int PlayersNumber = 1;

        public static readonly Dictionary<string, Keys> PlayerKeyboardInputs = new Dictionary<string, Keys>()
        {
            {"Up", Keys.Z },
            {"Right", Keys.D },
            {"Down", Keys.S },
            {"Left", Keys.Q },
            {"Slow", Keys.LeftShift },
        };

        public static readonly Buttons[] PlayerGamepadInput =
        {
            Buttons.LeftTrigger, Buttons.RightTrigger
        };

        public static Controller[] PlayersController =
        {
            Controller.Keyboard,
            Controller.GamePad
        };

        public static readonly TimeSpan PlayerTimeBeforeRespawn = TimeSpan.FromSeconds(1);
        public static readonly TimeSpan PlayerInvicibleTime = TimeSpan.FromSeconds(10);
        public static readonly TimeSpan DefaultBulletTimeTimer = TimeSpan.FromSeconds(1);
        public static readonly TimeSpan PlayerShootFrequency = TimeSpan.FromMilliseconds(1);
        public const float PlayerMaxVelocity = 800f;
        public const float PlayerMaxSlowVelocity = 125f;
        public static Vector2 PlayerBulletVelocity = new Vector2(1000f, 1000f);
        public const int PlayerLives = 5;

        // Camera
        public static readonly Vector2 CameraDistanceFromPlayer = new Vector2(
            Resolution.X/ 3f, 
            Resolution.Y / 3f
        );
        public const float CameraMotionInterpolationAmount = 0.05f;
        public const float CameraZoomInterpolationAmount = 0.05f;
        public const float CameraZoomLimit = 3f;
        public const float CameraZoomMin = 0.3f;
        public const float CameraZoomMax = 1f;
        public const bool DisableCameraZoom = true;

        // Boss
        public static float MinBossPartArea = 10000f;
        public static int MinBossIteration = 5;

        // GameRef
        public static readonly Point GameArea = new Point(2400, 1800);

        public const float GameDifficulty = 0.5f;

        public static float GameDifficultyDelegate()
        {
            return GameDifficulty;
        }

        public static TimeSpan BossInitialTimer = TimeSpan.FromSeconds(5);

        // Audio
        public static int SoundVolume = 1;
        public static int MusicVolume = 1;

        // Random
        public static int RandomSeed = 42;
    }
}
