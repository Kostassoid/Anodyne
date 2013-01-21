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

namespace Kostassoid.Anodyne.Node.Dependency
{
    /// <summary>
    /// Container component lifestyle.
    /// </summary>
    public enum Lifestyle
    {
        /// <summary>
        /// Default provider lifestyle.
        /// </summary>
        ProviderDefault,
        /// <summary>
        /// Resolved objects aren't managed and get garbage-collected when they're not referenced anymore.
        /// </summary>
        Unmanaged,
        /// <summary>
        /// Singleton. Only one instance of component exists.
        /// </summary>
        Singleton,
        /// <summary>
        /// New instance is created upon every resolving but explicit release is required.
        /// </summary>
        Transient,
        /// <summary>
        /// Resolved instance will be disposed upon web request end.
        /// </summary>
        PerWebRequest
    }
}