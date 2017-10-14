using DanmakuNoKyojin.Controls;
using DanmakuNoKyojin.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DanmakuNoKyojin.Screens
{
    public abstract partial class GameScreen : GameState
    {
        protected ControlManager ControlManager;

        public GameScreen(GameStateManager manager)
            : base(manager)
        {
        }

        public override void LoadContent(IContentLoader provider)
        {
            var controlFont = provider.Load<SpriteFont>("Graphics/Fonts/ControlFont");
            ControlManager = new ControlManager(controlFont);

            base.LoadContent(provider);
        }
    }
}
