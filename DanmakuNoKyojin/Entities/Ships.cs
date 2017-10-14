using DanmakuNoKyojin.Camera;
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
        private readonly Camera2D camera;

        public Ships(Camera2D camera)
        {
            this.camera = camera;
        }

        public void LoadContent(IContentLoader provider)
        {
            enemy = new ShipView(Program.system);
            enemy.LoadContent(provider);
        }

        public void Update(GameTime gameTime)
        {
            enemy.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.GetTransformation());

            enemy.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }
    }
}
