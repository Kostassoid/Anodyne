// Copyright 2011-2013 Anodyne.
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

namespace Kostassoid.Anodyne.Abstractions.DataAccess
{
    using System;

    [Serializable]
    public class PersistableRoot : IPersistableRoot
    {
        //TODO: bad
        object IPersistableRoot.IdObject
        {
            get { throw new NotSupportedException("Not supported for PersistableRoot-derived classes. Use PersistableRoot<> instead."); }
        }

	    public long Version { get; private set; }

	    public long BumpVersion()
	    {
		    return Version++;
	    }

	    public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;

            var thisEntity = this as IPersistableRoot;
            var thatEntity = (IPersistableRoot)obj;

            return thisEntity.IdObject.Equals(thatEntity.IdObject);
        }

        public override int GetHashCode()
        {
            return ((IPersistableRoot)this).IdObject.GetHashCode();
        }
    }

    [Serializable]
    public class PersistableRoot<TKey> : PersistableRoot, IPersistableRoot, IdentifiedBy<TKey>
    {
        object IPersistableRoot.IdObject { get { return Id; } }

        public TKey Id { get; protected set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;

            var thisEntity = this as IPersistableRoot;
            var thatEntity = (IPersistableRoot)obj;

            return thisEntity.IdObject.Equals(thatEntity.IdObject);
        }

        public override int GetHashCode()
        {
            return ((IPersistableRoot)this).IdObject.GetHashCode();
        }
    }
}