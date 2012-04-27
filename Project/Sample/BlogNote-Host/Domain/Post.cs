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
    using System;
    using Anodyne.Common;
    using Anodyne.Common.CodeContracts;
    using Anodyne.Common.Tools;
    using Anodyne.Domain.Base;

    public class Post : AggregateRoot<Guid>
    {
        public Guid User { get; protected set; }

        public PostContents Contents { get; protected set; }

        public DateTime Created { get; protected set; }
        public DateTime? Updated { get; protected set; }

        #region Creation

        protected Post()
        {
            Id = SeqGuid.NewGuid();
        }

        public static Post Create(PostContents contents)
        {
            Requires.NotNull(contents, "contents");

            var post = new Post();
            post.Apply(new PostCreatedEvent(post, contents));
            return post;
        }

        protected void OnPostCreated(PostCreatedEvent @event)
        {
            Contents = @event.Data.Contents;

            Created = SystemTime.Now;
        }

        #endregion

        #region Update

        public void Update(PostContents contents)
        {
            Requires.NotNull(contents, "contents");

            Apply(new PostUpdatedEvent(this, contents));
        }

        protected void OnUpdated(PostUpdatedEvent @event)
        {
            Contents = @event.Data.Contents;

            Updated = SystemTime.Now;
        }

        #endregion
         
    }
}