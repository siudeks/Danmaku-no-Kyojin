using DanmakuNoKyojin.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DanmakuNoKyojin.Entities
{
    /// <summary>
    /// Contains all visible ships.
    /// </summary>
    public sealed class Ships : IContentBasedPart, IUpdatablePart, IDrawablePart
    {

        ShipView enemy;

        public void LoadContent(IContentLoader provider)
        {
            enemy = new ShipView(Program.system);
            enemy.LoadContent(provider);
            enemy.ChangeDirection(new Vector2(100, 100), 0);
        }

        public void Update(GameTime gameTime)
        {
            enemy.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spiteBatch)
        {
            enemy.Draw(gameTime, spiteBatch);
        }
    }
}
