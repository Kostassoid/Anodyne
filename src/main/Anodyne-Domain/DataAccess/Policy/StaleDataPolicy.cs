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

namespace Kostassoid.Anodyne.Domain.DataAccess.Policy
{
    /// <summary>
    /// Defines behavior when there's a version mismatch between stored and changed AggregateRoot.
    /// </summary>
    public enum StaleDataPolicy
    {
        /// <summary>
        /// Doesn't overwrite conflicting roots. Throws StaleDataException. (default)
        /// </summary>
        Strict,
        /// <summary>
        /// Skips conflicting roots without throwing exception.
        /// </summary>
        SilentlySkip,
        /// <summary>
        /// Overwrites conflicting roots. Not compatible with Event Sourcing.
        /// </summary>
        Ignore
    }
}