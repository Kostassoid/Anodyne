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

namespace Kostassoid.Anodyne.Node.Dependency.Registration
{
    using Common;
    using Dependency;
    using System;

    public interface ISingleBindingSyntax : IBindingSyntax
    {
        ISingleBindingSyntax Use<TImpl>() where TImpl : class;
        ISingleBindingSyntax Use(Func<object> bindingFunc);
        ISingleBindingSyntax UseSelf();
        ISingleBindingSyntax UseInstance<TImpl>(TImpl instance) where TImpl : class;
        ISingleBindingSyntax With(Lifestyle lifestyle);
        ISingleBindingSyntax Named(string name);
    }

    public interface ISingleBindingSyntax<in TService> : IBindingSyntax where TService : class
    {
        ISingleBindingSyntax<TService> Use<TImpl>() where TImpl : TService;
        ISingleBindingSyntax<TService> Use(Func<TService> bindingFunc);
        ISingleBindingSyntax<TService> UseSelf();
        ISingleBindingSyntax<TService> UseInstance<TImpl>(TImpl instance) where TImpl : class, TService;
        ISingleBindingSyntax<TService> With(Lifestyle lifestyle);
        ISingleBindingSyntax<TService> Named(string name);
    }
}