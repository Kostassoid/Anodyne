using MongoDB.Driver;

namespace Kostassoid.Anodyne.DataAccess.MongoDb
{
    public class DataSessionFactory: IDataSessionFactory
    {
        protected string DatabaseName { get; private set; }
        protected MongoServer Server { get; private set; }
        protected IOperationResolver OperationResolver { get; private set; }

        public DataSessionFactory(string connectionString, string databaseName, IOperationResolver operationResolver)
        {
            OperationResolver = operationResolver;

            DatabaseName = databaseName;

            Server = MongoServer.Create(connectionString);
        }

        virtual public IDataSession OpenSession()
        {
            //SafeMode is important!
            return new DataSession(Server.GetDatabase(DatabaseName, SafeMode.True), OperationResolver); //reusing database connection (ref: http://www.mongodb.org/display/DOCS/CSharp+Driver+Tutorial#CSharpDriverTutorial-Threadsafety)
        }
    }
}