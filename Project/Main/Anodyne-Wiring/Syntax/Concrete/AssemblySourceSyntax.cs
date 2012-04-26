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

namespace Kostassoid.Anodyne.Wiring.Syntax.Concrete
{
    using System;
    using System.Reflection;
    using Subscription;

    internal class AssemblySourceSyntax<TEvent> : IAssemblySourceSyntax<TEvent> where TEvent : class, IEvent
    {
        private readonly SubscriptionSpecification<TEvent> _specification;

        public AssemblySourceSyntax(SubscriptionSpecification<TEvent> specification)
        {
            _specification = specification;
        }

        public ISourceTypeFilterSyntax<TEvent> FromThisAssembly()
        {
            _specification.SourceAssembly = new AssemblySpecification(Assembly.GetCallingAssembly());

            return new SourceTypeFilterSyntax<TEvent>(_specification);
        }

        public ISourceTypeFilterSyntax<TEvent> From(Predicate<string> assemblyNameFilter)
        {
            _specification.SourceAssembly = new AssemblySpecification(assemblyNameFilter);

            return new SourceTypeFilterSyntax<TEvent>(_specification);
        }
    }
}