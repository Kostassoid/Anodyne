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

namespace Kostassoid.Anodyne.Common.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

	public static class TypeEx
    {
		public delegate void HandlerDelegate(object instance, object param);

        public static bool IsSubclassOfRawGeneric(this Type type, Type generic)
        {
            while (type != null & type != typeof(object))
            {
                var intermediate = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (generic == intermediate)
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }

        public static bool IsRawGeneric(this Type type, Type generic)
        {
            if (type == null || generic == null) return false;

            return type.IsGenericType && type.GetGenericTypeDefinition() == generic;
        }

		public static IEnumerable<MethodInfo> FindMethodHandlers(this Type instanceType, Type paramType, bool isPolymorphic)
		{
			return instanceType
				.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(mi => IsTargetCompatibleWithSource(mi, paramType, isPolymorphic))
				.ToList();
		}

		private static bool IsTargetCompatibleWithSource(MethodInfo methodInfo, Type paramType, bool isPolymorphic)
		{
			if (methodInfo.ReturnType != typeof(void)) return false;

			var parameters = methodInfo.GetParameters();
			if (parameters.Length != 1) return false;

			if (isPolymorphic)
			{
				return parameters[0].ParameterType.IsAssignableFrom(paramType);
			}

			return parameters[0].ParameterType == paramType;
		}

		public static HandlerDelegate BuildMethodHandler(MethodInfo methodInfo, Type paramType)
		{
			var instance = Expression.Parameter(typeof(object), "instance");
			var param = Expression.Parameter(typeof(object), "param");

			var lambda = Expression.Lambda<HandlerDelegate>(
				Expression.Call(
					Expression.Convert(instance, methodInfo.DeclaringType),
					methodInfo,
					Expression.Convert(param, paramType)
					),
				instance,
				param
				);

			return lambda.Compile();
		}
	}
}