using System;
using System.Linq;
using Kostassoid.Anodyne.Domain.Base;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Wrappers;

namespace Kostassoid.Anodyne.DataAccess.MongoDb
{
    public class MongoHelper
    {
        public static void EnsureCappedCollectionExists<T>(int collectionSizeMb) where T : IAggregateRoot
        {
            if (UnitOfWork.Current.IsNone)
                throw new InvalidOperationException("Must be inside UnitOfWork context");

            var database = ((IDataSessionEx)UnitOfWork.Current.Value.DataSession).NativeSession as MongoDatabase;
            var collectionName = typeof(T).Name;
            var collectionSize = collectionSizeMb * 1024 * 1024;

            if (database.CollectionExists(collectionName))
            {
                if (!database.GetCollection<T>(collectionName).IsCapped())
                {
                    var result = database.RunCommand(new CommandWrapper(new { convertToCapped = collectionName, size = collectionSize }));
                    if (!result.Ok)
                    {
                        throw new Exception(string.Format("Unable to convert collection {0} to capped! Result: {1}", collectionName, result.ErrorMessage));
                    }
                }
            }

            if (!database.CollectionExists(collectionName))
            {
                var options = CollectionOptions.SetCapped(true);
                options.SetMaxSize(collectionSize);
                database.CreateCollection(collectionName, options);
            }

        }

        public static void CreateMapForAllClassesBasedOn<TBase>(Predicate<string> assemblyNamePredicate)
        {
            BsonClassMap.RegisterClassMap<TBase>(cm =>
                                                {
                                                    cm.AutoMap();
                                                    cm.SetDiscriminatorIsRequired(true);
                                                    cm.SetIsRootClass(true);
                                                });


            var types = AppDomain.CurrentDomain.GetAssemblies().Where(a => assemblyNamePredicate(a.FullName)).ToList()
                .SelectMany(s => s.GetTypes())
                .Where(typeof(TBase).IsAssignableFrom).Where(t => !t.ContainsGenericParameters);

            foreach (var type in types)
            {
                if (BsonClassMap.IsClassMapRegistered(type)) continue;
                BsonClassMap.LookupClassMap(type);
            }
        }
    }
}
