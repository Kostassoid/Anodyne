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

namespace Kostassoid.Anodyne.Common
{
    using System;
    using Extentions;

    public abstract class Option<T> where T : class
    {
        public static Option<T> None
        {
            get { return new None<T>(); }
        }

        public static Option<T> Some(T value)
        {
            return new Some<T>(value);
        }

        public abstract T Value { get; }
        public abstract bool IsSome { get; }
        public abstract bool IsNone { get; }

        public static implicit operator Option<T>(T value)
        {
            return value.AsOption();
        }

        public static implicit operator bool(Option<T> option)
        {
            return option.IsSome;
        }

        public static explicit operator T(Option<T> option)
        {
            return option.Value;
        }
    }

    public sealed class Some<T> : Option<T> where T : class
    {
        private readonly T _value;

        public Some(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "Some value was null, use None instead");
            }

            if (typeof(T).IsSubclassOfRawGeneric(typeof(Option<>)))
            {
                throw new InvalidOperationException("Nested Option is not supported!");
            }

            _value = value;
        }

        public override T Value
        {
            get { return _value; }
        }

        public override bool IsSome
        {
            get { return true; }
        }

        public override bool IsNone
        {
            get { return false; }
        }
    }

    public sealed class None<T> : Option<T> where T : class
    {
        public override T Value
        {
            get { throw new NotSupportedException("There is no value"); }
        }

        public override bool IsSome
        {
            get { return false; }
        }

        public override bool IsNone
        {
            get { return true; }
        }
    }

    public static class OptionEx
    {
        public static Option<T> AsOption<T>(this T value) where T : class
        {
            if (value == null) return new None<T>();

            return new Some<T>(value);
        }
    }
}