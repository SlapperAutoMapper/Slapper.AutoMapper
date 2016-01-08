using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Slapper.Tests
{
    [TestFixture]
    public class ComplexMapsParentsAndChlidTest: TestBase
    {
        public class Hotel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Tour
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Service
        {
            public int Id { get; set; }
            public IEnumerable<Hotel> Hotels { get; set; }
            public IEnumerable<Tour> Tours { get; set; }
        }

        public class Booking
        {
            public int Id { get; set; }
            public IEnumerable<Service> Services { get; set; }
        }

        [Test]
        public void Can_Make_Cache_HashTypeEquals_With_Diferents_Parents()
        {
            var listOfDictionaries = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "Id", 1 },
                    { "Services_Id", 1 },
                    { "Services_Hotels_Id", 1 },
                    { "Services_Hotels_Name", "Hotel 1" }
                },
                new Dictionary<string, object>
                {
                    { "Id", 1 },
                    { "Services_Id", 2 },
                    { "Services_Hotels_Id", 2 },
                    { "Services_Hotels_Name", "Hotel 2" }
                },
                new Dictionary<string, object>
                {
                    { "Id", 2 },
                    { "Services_Id", 1 },
                    { "Services_Hotels_Id", 3 },
                    { "Services_Hotels_Name", "Hotel 3" }
                }
            };

            var bookings = AutoMapper.Map<Booking>(listOfDictionaries).ToList();

            Assert.That(bookings.Count == 2);
            Assert.That(bookings[0].Services.Count() == 2);

            Assert.NotNull(bookings[0].Services.SingleOrDefault(s => s.Id == 1));
            Assert.That(bookings[0].Services.SingleOrDefault(s => s.Id == 1).Hotels.Count() == 1);
            Assert.That(bookings[0].Services.SingleOrDefault(s => s.Id == 2).Hotels.Count() == 1);

            Assert.That(bookings[1].Services.Count() == 1);

            Assert.NotNull(bookings[1].Services.SingleOrDefault(s => s.Id == 1));
            Assert.That(bookings[1].Services.SingleOrDefault(s => s.Id == 1).Hotels.Count() == 1);
        }
    }
}
