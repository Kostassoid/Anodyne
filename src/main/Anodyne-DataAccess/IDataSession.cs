using System;

namespace Kostassoid.Anodyne.DataAccess
{
    public interface IDataSession : IDataSessionEx, IDisposable
    {
        object FindOne(Type type, object id);
        void SaveOne(Type type, object o);
        void RemoveOne(Type type, object id);
    }
}