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

namespace Kostassoid.Anodyne.Common.Specs
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class TicketSpecs
    {

        [TestFixture]
        [Category("Unit")]
        public class when_trying_to_validate_ticket
        {
            [Test]
            public void should_validate_ticket_according_to_current_datetime()
            {
                var ticket = Ticket.Generate(TimeSpan.FromHours(2));

                ticket.Length.Should().BeGreaterThan(15);

                Ticket.HasExpired(ticket).Should().BeFalse();

                SystemTime.TimeController.SetDate(SystemTime.Now.AddMinutes(121));

                Ticket.HasExpired(ticket).Should().BeTrue();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_trying_to_generate_many_tickets
        {
            [Test]
            public void generated_tickets_should_be_unique()
            {
                var tickets = Enumerable.Range(0, 1000).Select(_ => Ticket.Generate(TimeSpan.FromHours(2)));

                tickets.Should().OnlyHaveUniqueItems();
            }
        }
    }
    // ReSharper restore InconsistentNaming

}
