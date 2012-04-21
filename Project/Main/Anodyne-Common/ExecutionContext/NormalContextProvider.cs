using System.Runtime.Remoting.Messaging;

namespace Kostassoid.Anodyne.Common.ExecutionContext
{
    public class NormalContextProvider : IContextProvider
    {
        public void Set(string name, object value)
        {
            CallContext.SetData(name, value);
        }

        public object Find(string name)
        {
            return CallContext.GetData(name);
        }

        public void Release(string name)
        {
            CallContext.FreeNamedDataSlot(name);
        }
    }
}