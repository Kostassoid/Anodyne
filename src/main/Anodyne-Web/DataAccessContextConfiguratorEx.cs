namespace Kostassoid.Anodyne.Web
{
    using System;
    using Abstractions.DataAccess;

    public static class DataAccessContextConfiguratorEx
    {
        public static void BoundToWebRequest(this DataAccessContextConfigurator cc)
        {
            var node = Node.Node.Current;
            if (!(node is WebNode))
            {
                throw new InvalidOperationException(string.Format("Expected Node to be of WebNode type but was {0}", node.GetType().Name));
            }

            var container = node.Configuration.Container;
            ((WebNode)node).Application.EndRequest += (sender, args) =>
            {
                if (container.Has<IDataAccessContext>())
                    container.Get<IDataAccessContext>().CloseCurrentSession();
            };
        }
    }
}