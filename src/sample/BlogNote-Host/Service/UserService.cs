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

namespace Kostassoid.BlogNote.Host.Service
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using Anodyne.Common.Extentions;
    using Anodyne.Domain.DataAccess.RootOperation;
    using Contracts;
    using Domain;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class UserService : IUserService
    {
        private static void RequireUserExists(Guid user)
        {
			OnRoot<User>
				.IdentifiedBy(user)
				.Perform(u =>
					{
						if (u.IsNull())
							throw new ArgumentException("Can't find user with id {0}.".FormatWith(user), "user");
					});
        }

        public Guid EnsureUserExists(string name, string email)
        {
			return OnRoot<User>
				.AcquiredBy(q => q.FirstOrDefault(u => u.Name == name))
				.Request(user => user.IsNotNull() ? user.Id : User.Create(name, email).Id);
        }

        public Guid PostText(Guid user, string title, string body, string[] tags)
        {
            RequireUserExists(user);

			return OnRoot<Post>
				.ConstructedBy(() => Post.Create(new TextContent(title, body, tags)))
				.Request(post => post.Id);
        }

        public Guid PostUrl(Guid user, string title, string url, string[] tags)
        {
            RequireUserExists(user);

			return OnRoot<Post>
				.ConstructedBy(() => Post.Create(new UrlContent(title, url, tags)))
				.Request(post => post.Id);
        }
    }
}