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
    using System;
    using MongoDB.Bson;

    internal static class ObjectEx
    {
        public static BsonValue AsIdValue(this object id)
        {
            if (id is Guid) return (Guid)id;
            if (id is string) return (string)id;
            if (id is int) return (int)id;
            if (id is long) return (long)id;
            if (id is ObjectId) return (ObjectId)id;

            throw new InvalidOperationException(string.Format("Unsupported _id type : {0}", id.GetType().Name));
        }
         
    }
}