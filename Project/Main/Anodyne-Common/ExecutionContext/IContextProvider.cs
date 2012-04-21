namespace Kostassoid.Anodyne.Common.ExecutionContext
{
    public interface IContextProvider
    {
        void Set(string name, object value);
        object Find(string name);
        void Release(string name);
    }
}