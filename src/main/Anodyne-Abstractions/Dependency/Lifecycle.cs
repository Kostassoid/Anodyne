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

namespace Kostassoid.Anodyne.Abstractions.Dependency
{
    /// <summary>
    /// Container component Lifecycle.
    /// </summary>
    public class Lifecycle
    {
        /// <summary>
        /// Default Lifecycle (singleton).
        /// </summary>
        public static Lifecycle Default = new Lifecycle("Default");
        /// <summary>
        /// Default provider Lifecycle.
        /// </summary>
        public static Lifecycle ProviderDefault = new Lifecycle("ProviderDefault");
        /// <summary>
        /// Resolved objects aren't managed and get garbage-collected when they're not referenced anymore.
        /// </summary>
        public static Lifecycle Unmanaged = new Lifecycle("Unmanaged");
        /// <summary>
        /// Singleton. Only one instance of component exists.
        /// </summary>
        public static Lifecycle Singleton = new Lifecycle("Singleton");
        /// <summary>
        /// New instance is created upon every resolving but explicit release may be required.
        /// </summary>
        public static Lifecycle Transient = new Lifecycle("Transient");
        /// <summary>
        /// Resolved instance will be disposed upon web request end.
        /// </summary>
        public static Lifecycle PerWebRequest = new Lifecycle("PerWebRequest");

        /// <summary>
        /// Use provider-specific or user-defined Lifecycle.
        /// </summary>
        /// <param name="name">Lifecycle name.</param>
        /// <returns>Custom Lifecycle descriptor.</returns>
        public static Lifecycle Custom(string name)
        {
            return new Lifecycle(name);
        }

        /// <summary>
        /// Lifecycle name.
        /// </summary>
        public string Name { get; private set; }

        private Lifecycle(string name)
        {
            Name = name;
        }

        protected bool Equals(Lifecycle other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Lifecycle) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}