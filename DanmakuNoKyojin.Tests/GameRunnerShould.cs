using NUnit.Framework;

namespace DanmakuNoKyojin
{
    [TestFixture]
    public sealed class GameRunnerShould
    {
        [Test]
        public void BeInstantiable()
        {
            new GameRunner();
        }
    }
}
