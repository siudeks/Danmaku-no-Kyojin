namespace DanmakuNoKyojin.Framework
{
    public interface IContentLoader
    {
        T Load<T>(string assetName);
    }
}
