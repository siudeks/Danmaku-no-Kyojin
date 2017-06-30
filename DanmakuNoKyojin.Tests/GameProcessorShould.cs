using NUnit.Framework;

namespace DanmakuNoKyojin
{
    [TestFixture]
    public sealed class GameProcessorShould
    {
        [Test]
        public void BeInstantiable()
        {
            new GameProcessor();
        }
    }
}
