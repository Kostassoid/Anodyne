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

namespace Kostassoid.Anodyne.Common.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Kostassoid.Anodyne.Common.Tools;

    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class ToolsSpecs
    {

        [TestFixture]
        [Category("Unit")]
        public class when_generating_many_seq_guids
        {

            [Test]
            public void they_should_be_sequental_and_unique()
            {
                var generatedSet = new List<Guid>();

                var i = 100;
                while (i --> 0)
                {
                    generatedSet.Add(SeqGuid.NewGuid());
                }

                Assert.That(new HashSet<Guid>(generatedSet).Count, Is.EqualTo(100));
                Assert.That(generatedSet.OrderBy(g => g), Is.EquivalentTo(generatedSet));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_generating_many_seq_guids_async
        {
            [Test]
            [Ignore("They're not unique, and .NET Guid is not unique, life's a pain")]
            public void they_should_be_unique()
            {
                var generatedSet = new List<Guid>();
                var tasks = new List<Task>();

                var i = 1000;
                while (i --> 0)
                {
                    tasks.Add(Task.Factory.StartNew(() => generatedSet.Add(SeqGuid.NewGuid())));
                }

                Task.WaitAll(tasks.ToArray());

                Assert.That(new HashSet<Guid>(generatedSet).Count, Is.EqualTo(1000));
            }
        }
    }
    // ReSharper restore InconsistentNaming

}
