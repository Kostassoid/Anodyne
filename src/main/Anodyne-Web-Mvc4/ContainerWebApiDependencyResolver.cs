using System;
using System.Collections.Generic;
using System.Linq;
using Kostassoid.Anodyne.Abstractions.Dependency;

namespace Kostassoid.Anodyne.Web.Mvc4
{
    public class ContainerWebApiDependencyResolver : System.Web.Http.Dependencies.IDependencyResolver 
    {
        private readonly IContainer _container;

        public ContainerWebApiDependencyResolver(IContainer container)
        {
            _container = container;
        }
        
        public System.Web.Http.Dependencies.IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            return _container.Has(serviceType) ? _container.Get(serviceType) : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.Has(serviceType) ? _container.GetAll(serviceType).Cast<object>() : new object[] { };
        }

        public void Dispose(){}
    }
}
