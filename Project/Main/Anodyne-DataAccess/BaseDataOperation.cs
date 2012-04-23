using System;

namespace Kostassoid.Anodyne.DataAccess
{
    public abstract class BaseDataOperation
    {
        protected UnitOfWork Owner { get; private set; }

        protected BaseDataOperation()
        {
            if (UnitOfWork.Current.IsNone)
            {
                throw new Exception("Should be within UnitOfWork context!");
            }

            Owner = UnitOfWork.Current.Value;
        }

    }
}