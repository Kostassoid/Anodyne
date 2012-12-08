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

namespace Kostassoid.BlogNote.Host.Domain
{
    using Anodyne.Common;
    using Anodyne.Common.CodeContracts;
    using Anodyne.Common.Tools;
    using Anodyne.Domain.Base;
    using System;
    using Event;

    public class Post : AggregateRoot<Guid>
    {
        public Guid User { get; protected set; }

        public BasePostContent Content { get; protected set; }

        public DateTime Created { get; protected set; }
        public DateTime? Updated { get; protected set; }

        #region Creation

        protected Post()
        {
            Id = SeqGuid.NewGuid();
        }

        public static Post Create(BasePostContent content)
        {
            Requires.NotNull(content, "Content");

            var post = new Post();
            Apply(new PostCreated(post, content));
            return post;
        }

        protected void OnCreated(PostCreated @event)
        {
            Id = @event.Target.Id;

            Content = @event.Content;

            Created = SystemTime.Now;
        }

        #endregion

        #region Update

        public void Update(BasePostContent content)
        {
            Requires.NotNull(content, "Content");

            Apply(new PostUpdated(this, content));
        }

        protected void OnUpdated(PostUpdated @event)
        {
            Content = @event.Content;

            Updated = SystemTime.Now;
        }

        #endregion
         
    }
}