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

namespace Kostassoid.Anodyne.Common.Extentions
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public static class ObjectEx
    {
        public static IEnumerable<T> AsEnumerable<T>(this T obj)
        {
			// ReSharper disable CompareNonConstrainedGenericWithNull
            if (obj != null)
			// ReSharper restore CompareNonConstrainedGenericWithNull
                yield return obj;
        }

        public static IEnumerable<T> AsEnumerable<T>(this object obj)
        {
            if (obj is T)
                yield return (T)obj;
        }

        public static T DeepClone<T>(this T obj)
        {
            return (T)obj.DeepClone(typeof(T));
        }

        public static T DeepCloneAs<T>(this T obj, Type type)
        {
            return (T)obj.DeepClone(type);
        }

        public static object DeepClone(this object obj, Type type)
        {
            var members = FormatterServices.GetSerializableMembers(type);
            var data = FormatterServices.GetObjectData(obj, members);
            var cloned = FormatterServices.GetSafeUninitializedObject(type);
            FormatterServices.PopulateObjectMembers
                (cloned, members, data);
            return cloned;
        }

		/// <summary>
		/// Checks if the object is null.
		/// </summary>
		public static bool IsNull(this object obj)
		{
			return obj == null;
		}

		/// <summary>
		/// Checks if the object is not null.
		/// </summary>
		public static bool IsNotNull(this object obj)
		{
			return obj != null;
		}


    }
}