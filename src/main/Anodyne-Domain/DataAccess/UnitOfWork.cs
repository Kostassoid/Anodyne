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

namespace Kostassoid.Anodyne.Domain.DataAccess
{
    using Common;
    using Policy;

    public static class UnitOfWork
    {
        private static DataAccessPolicy _globalPolicy = new DataAccessPolicy();
        public static DataAccessPolicy GlobalPolicy
        {
            get { return _globalPolicy; }
            set
            {
                _globalPolicy = value;
                if (Manager != null)
                    Manager.Policy = _globalPolicy;
            }
        }

        public static IUnitOfWorkManager Manager { get; private set; }

        public static Option<IUnitOfWork> Root
        {
            get { return Manager.Root; }
        }

        public static Option<IUnitOfWork> Head
        {
            get { return Manager.Head; }
        }

        public static bool IsConfigured
        {
            get { return Manager != null; }
        }

        static UnitOfWork()
        {
            GlobalPolicy = new DataAccessPolicy();
        }

        public static IUnitOfWork Start(StaleDataPolicy? staleDataPolicy = null)
        {
            return Manager.Start(staleDataPolicy);
        }

        public static void Initialize(IUnitOfWorkManager manager)
        {
            Reset();

            Manager = manager;
            Manager.Policy = _globalPolicy;
        }

        public static void Reset()
        {
            if (Manager != null)
            {
                Manager.Dispose();
                Manager = null;
            }
        }
    }
}