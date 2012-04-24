using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Kostassoid.Anodyne.DataAccess.MongoDb
{
    public static class MongoCollectionEx
    {
        //TODO: rethink probably
        public static void EnsureIndex<T, U>(this MongoCollection<T> collection, Expression<Func<T, U>> index, bool isUnique, bool ascending, bool isSparse, string indexName = null)
        {
            var exp = index.Body as NewExpression;
            var keys = new HashSet<string>();
            if (exp != null)
            {
                foreach (var x in exp.Arguments.OfType<MemberExpression>())
                {
                    keys.Add(GetPropertyAlias(x));
                }
            }
            else if (index.Body is MemberExpression)
            {
                var me = index.Body as MemberExpression;
                keys.Add(GetPropertyAlias(me));
            }

            var keysCombined = String.Join(",", keys);
            var indexKey = ascending ?
                IndexKeys.Ascending(keysCombined) :
                IndexKeys.Descending(keysCombined);

            var indexOptions = IndexOptions.SetName(String.IsNullOrEmpty(indexName) ? keysCombined + "_" : indexName).SetUnique(isUnique);

            collection.EnsureIndex(indexKey, indexOptions);
        }

        public static void EnsureIndex<T, U>(this MongoCollection<T> collection, Expression<Func<T, U>> index, bool isUnique, bool ascending, string indexName = null)
        {
            collection.EnsureIndex(index, isUnique, ascending, false, indexName);
        }

        public static void EnsureSparseIndex<T, U>(this MongoCollection<T> collection, Expression<Func<T, U>> index, bool isUnique, bool ascending, string indexName = null)
        {
            collection.EnsureIndex(index, isUnique, ascending, true, indexName);
        }

        //TODO: heavy test this
        public static string GetPropertyAlias(MemberExpression mex)
        {
            var retval = "";
            var parentEx = mex.Expression as MemberExpression;
            if (parentEx != null)
            {
                //we need to recurse because we're not at the root yet.
                retval += GetPropertyAlias(parentEx) + ".";
            }
            //retval += MongoConfiguration.GetPropertyAlias(mex.Expression.Type, mex.Member.Name);
            retval += mex.Member.Name;
            return retval;
        }

    }
}