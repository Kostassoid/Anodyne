using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Kostassoid.Anodyne.Common.Specs
{
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
