using System;

namespace Kostassoid.Anodyne.DataAccess
{
    public class EntityNotFoundException : Exception
    {
        public object Key { get; protected set; }
        public EntityNotFoundException(object key)
        {
            Key = key;
        }
    }
}