// Copyright 2011-2012 Anodyne.
//   
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
//  
//      http://www.apache.org/licenses/LICENSE-2.0 
//  
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

namespace Kostassoid.Anodyne.MongoDb
{
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Linq.Expressions;

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
            var indexKey = ascending
                               ? IndexKeys.Ascending(keysCombined)
                               : IndexKeys.Descending(keysCombined);

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