namespace DanmakuNoKyojin.Framework
{
    /// <summary>
    /// Emitted once with application
    /// 
    /// Contains initial info about game environment
    /// </summary>
    public sealed class FrameworkInitialized
    {
        public readonly float ViewportWidth;
        public readonly float ViewportHeight;
    }
}
