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

namespace Kostassoid.Anodyne.Domain.Base
{
    using System;

    [Serializable]
    public abstract class Entity : IEntity
    {
        //TODO: bad
        object IEntity.IdObject
        {
            get { throw new NotSupportedException("Not supported for Entity-derived classes. Use Entity<> instead."); }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;

            var thisEntity = this as IEntity;
            var thatEntity = (IEntity)obj;

            return thisEntity.IdObject.Equals(thatEntity.IdObject);
        }

        public override int GetHashCode()
        {
            return (this as IEntity).IdObject.GetHashCode();
        }
    }

    [Serializable]
    public abstract class Entity<TKey> : Entity, IEntity
    {
        public TKey Id { get; protected set; }

        object IEntity.IdObject
        {
            get { return Id; }
        }
    }
}