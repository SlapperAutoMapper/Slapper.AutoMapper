using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slapper.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class MapCollectionsTypedTest : TestBase
    {
        public class Customer
        {
            public int CustomerId;
            public IList<int> OrdersIds;
        }

        public class CustomerNames
        {
            public int CustomerNamesId;
            public IList<string> Names;
        }

        [Test]
        public void I_Can_Map_Value_PrimitiveTyped_Collection()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>
                         {
                             { "CustomerId", 1 },
                             { "OrdersIds_$", 3 },
                         };

            var dictionary2 = new Dictionary<string, object>
                         {
                             { "CustomerId", 1 },
                             { "OrdersIds_$", 5 }
                         };

            var list = new List<IDictionary<string, object>> { dictionary, dictionary2 };

            // Act
            var customers = Slapper.AutoMapper.MapDynamic<Customer>(list).ToList();

            // Assert

            // There should only be a single customer
            Assert.That(customers.Count == 1);

            // There should be two values in OrdersIds, with the correct values
            Assert.That(customers.Single().OrdersIds.Count == 2);
            Assert.That(customers.Single().OrdersIds[0] == 3);
            Assert.That(customers.Single().OrdersIds[1] == 5);
        }


        [Test]
        public void I_Can_Map_Value_SpecialStringTyped_Collection()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>
                         {
                             { "CustomerNamesId", 1 },
                             { "Names_$", "Name 1" },
                         };

            var dictionary2 = new Dictionary<string, object>
                         {
                             { "CustomerNamesId", 1 },
                             { "Names_$", "Name 2" }
                         };

            var list = new List<IDictionary<string, object>> { dictionary, dictionary2 };

            // Act
            var customers = Slapper.AutoMapper.MapDynamic<CustomerNames>(list).ToList();

            // Assert

            // There should only be a single customer
            Assert.That(customers.Count == 1);

            // There should be two values in OrdersIds, with the correct values
            Assert.That(customers.Single().Names.Count == 2);
            Assert.That(customers.Single().Names[0] == "Name 1");
            Assert.That(customers.Single().Names[1] == "Name 2");
        }

    }
}
