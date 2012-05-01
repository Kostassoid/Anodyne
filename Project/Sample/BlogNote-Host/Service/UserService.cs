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
// 

namespace Kostassoid.BlogNote.Host.Service
{
    using System;
    using System.Linq;
    using Anodyne.CommandBus;
    using Anodyne.DataAccess;
    using Contracts;
    using Domain;

    public class UserService : IUserService
    {
        public ICommandResult Send(ICommand command)
        {
            return null;
        }

        public Guid EnsureUserExists(string name, string email)
        {
            using (var uow = new UnitOfWork())
            {
                var foundUser = uow.Query<User>().All().FirstOrDefault(u => u.Name == name);
                if (foundUser != null) return foundUser.Id;

                return User.Create(name, email).Id;
            }
        }
    }
}