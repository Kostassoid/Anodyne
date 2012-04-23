using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Kostassoid.Anodyne.Common
{
    [Serializable]
    public abstract class SerializableValueObject : ICloneable
    {
        private const int HashMultiplier = 31;

        [ThreadStatic]
        private static Dictionary<Type, IList<MemberInfo>> _serializableMembers;

        protected virtual IList<MemberInfo> GetTypeSpecificSerializableMembers()
        {
            return FormatterServices.GetSerializableMembers(GetType());
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as SerializableValueObject;

            if (ReferenceEquals(this, compareTo))
                return true;

            return compareTo != null && GetType().Equals(compareTo.GetType()) &&
                HasSameObjectSignatureAs(compareTo);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var members = GetSerializableMembers();

                // It's possible for two objects to return the same hash code based on 
                // identically valued properties, even if they're of two different types, 
                // so we include the object's type in the hash calculation
                var hashCode = GetType().GetHashCode();

                var objectData = FormatterServices.GetObjectData(this, members.ToArray());

                foreach (var value in objectData)
                {
                    if (value != null)
                        hashCode = (hashCode * HashMultiplier) ^ value.GetHashCode();
                }

                if (members.Any())
                    return hashCode;

                // If no properties were flagged as being part of the signature of the object,
                // then simply return the hashcode of the base object as the hashcode.
                return base.GetHashCode();
            }
        }

        public object Clone()
        {
            var members = GetSerializableMembers().ToArray();
            var data = FormatterServices.GetObjectData(this, members);
            var cloned = FormatterServices.GetSafeUninitializedObject(GetType());
            FormatterServices.PopulateObjectMembers(cloned, members, data);
            return cloned;
        }

        public virtual bool HasSameObjectSignatureAs(SerializableValueObject compareTo)
        {
            if (compareTo.GetType() != GetType()) return false;

            var signatureProperties = GetSerializableMembers();

            var thisObjectData = FormatterServices.GetObjectData(this, signatureProperties.ToArray());
            var compareToObjectData = FormatterServices.GetObjectData(compareTo, signatureProperties.ToArray());

            for (var i = 0; i < signatureProperties.Count(); i++)
            {
                var valueOfThisObject = thisObjectData[i];
                var valueToCompareTo = compareToObjectData[i];

                if (valueOfThisObject == null && valueToCompareTo == null)
                    continue;

                if ((valueOfThisObject == null ^ valueToCompareTo == null) ||
                    (!valueOfThisObject.Equals(valueToCompareTo)))
                {
                    return false;
                }
            }

            // If we've gotten this far and signature properties were found, then we can
            // assume that everything matched; otherwise, if there were no signature 
            // properties, then simply return the default bahavior of Equals
            return signatureProperties.Any() || base.Equals(compareTo);
        }

        public virtual IList<MemberInfo> GetSerializableMembers()
        {
            IList<MemberInfo> properties;

            // Init the serializableMembers here due to reasons described at 
            // http://blogs.msdn.com/jfoscoding/archive/2006/07/18/670497.aspx
            if (_serializableMembers == null)
                _serializableMembers = new Dictionary<Type, IList<MemberInfo>>();

            if (_serializableMembers.TryGetValue(GetType(), out properties))
                return properties;

            return (_serializableMembers[GetType()] = GetTypeSpecificSerializableMembers());
        }
        
    }
}