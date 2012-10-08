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
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public static class MongoCollectionEx
    {
        //TODO: rethink probably
        public static void EnsureIndex<T, U>(this MongoCollection<T> collection, Expression<Func<T, U>> index, bool isUnique, bool ascending, bool isSparse, string indexName = null)
        {
            var keysCombined = string.Join(",", ResolveIndexKeysFrom(index));
            var indexKey = ascending
                               ? IndexKeys.Ascending(keysCombined)
                               : IndexKeys.Descending(keysCombined);

            var indexOptions = IndexOptions.SetName(String.IsNullOrEmpty(indexName) ? keysCombined + "_" : indexName).SetUnique(isUnique);

            collection.EnsureIndex(indexKey, indexOptions);
        }

        private static IEnumerable<string> ResolveIndexKeysFrom<T,U>(Expression<Func<T, U>> func)
        {
            return new[] { GetFullPropertyName(func) };
        }

        public static void EnsureIndex<T, U>(this MongoCollection<T> collection, Expression<Func<T, U>> index, bool isUnique, bool ascending, string indexName = null)
        {
            collection.EnsureIndex(index, isUnique, ascending, false, indexName);
        }

        public static void EnsureSparseIndex<T, U>(this MongoCollection<T> collection, Expression<Func<T, U>> index, bool isUnique, bool ascending, string indexName = null)
        {
            collection.EnsureIndex(index, isUnique, ascending, true, indexName);
        }

        // code adjusted to prevent horizontal overflow
        static string GetFullPropertyName<T, TProperty>
        (Expression<Func<T, TProperty>> exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp.Body, out memberExp))
                return string.Empty;

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }

        // code adjusted to prevent horizontal overflow
        private static bool TryFindMemberExpression(Expression exp, out MemberExpression memberExp)
        {
            memberExp = exp as MemberExpression;
            if (memberExp != null)
            {
                // heyo! that was easy enough
                return true;
            }

            // if the compiler created an automatic conversion,
            // it'll look something like...
            // obj => Convert(obj.Property) [e.g., int -> object]
            // OR:
            // obj => ConvertChecked(obj.Property) [e.g., int -> long]
            // ...which are the cases checked in IsConversion
            if (IsConversion(exp) && exp is UnaryExpression)
            {
                memberExp = ((UnaryExpression)exp).Operand as MemberExpression;
                if (memberExp != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsConversion(Expression exp)
        {
            return (
                exp.NodeType == ExpressionType.Convert ||
                exp.NodeType == ExpressionType.ConvertChecked
            );
        }
    }
}