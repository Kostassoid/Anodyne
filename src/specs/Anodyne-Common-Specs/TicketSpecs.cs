﻿// Copyright 2011-2012 Anodyne.
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

                Assert.IsTrue(ticket.Length > 15);

                Assert.IsFalse(Ticket.HasExpired(ticket));

                SystemTime.TimeController.SetDate(SystemTime.Now.AddMinutes(121));

                Assert.IsTrue(Ticket.HasExpired(ticket));

            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_trying_to_generate_many_tickets
        {
            [Test]
            public void generated_tickets_should_be_unique()
            {
                const int ticketsCount = 1000;
                ISet<string> tickets = new HashSet<string>();

                var ticketIndex = ticketsCount;
                while (ticketIndex-- > 0)
                {
                    tickets.Add(Ticket.Generate(TimeSpan.FromHours(2)));
                }

                Assert.AreEqual(ticketsCount, tickets.Count);

            }
        }
    }
    // ReSharper restore InconsistentNaming

}