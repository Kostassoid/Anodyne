namespace Kostassoid.Anodyne.DataAccess.Operations
{
    using System;
    using System.Dependency;

    public class ContainerOperationResolver : IOperationResolver
    {
        private readonly IContainer _container;

        public ContainerOperationResolver(IContainer container)
        {
            _container = container;
        }

        public TOp Get<TOp>() where TOp : IDataOperation
        {
            return _container.Get<TOp>();
        }
    }
}