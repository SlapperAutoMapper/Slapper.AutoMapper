using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Slapper.Tests
{
    [TestFixture]
    public class ReadMeTests : TestBase
    {
        public class Person
        {
            public int Id;
            public string FirstName;
            public string LastName;
        }

        public class Customer
        {
            public int CustomerId;
            public string FirstName;
            public string LastName;
            public IList<Order> Orders;
        }

        public class Order
        {
            public int OrderId;
            public decimal OrderTotal;
            public IList<OrderDetail> OrderDetails;
        }

        public class OrderDetail
        {
            public int OrderDetailId;
            public decimal OrderDetailTotal;
        }
        
        [Test]
        public void I_Can_Map_Nested_Types_And_Resolve_Duplicate_Entries_Properly()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>
                                 {
                                     { "CustomerId", 1 },
                                     { "FirstName", "Bob" },
                                     { "LastName", "Smith" },
                                     { "Orders_OrderId", 1 },
                                     { "Orders_OrderTotal", 50.50m },
                                     { "Orders_OrderDetails_OrderDetailId", 1 },
                                     { "Orders_OrderDetails_OrderDetailTotal", 25.00m }
                                 };

            var dictionary2 = new Dictionary<string, object>
                                 {
                                     { "CustomerId", 1 },
                                     { "FirstName", "Bob" },
                                     { "LastName", "Smith" },
                                     { "Orders_OrderId", 1 },
                                     { "Orders_OrderTotal", 50.50m },
                                     { "Orders_OrderDetails_OrderDetailId", 2 },
                                     { "Orders_OrderDetails_OrderDetailTotal", 25.50m }
                                 };

            var list = new List<IDictionary<string, object>> { dictionary, dictionary2 };

            // Act
            var customers = Slapper.AutoMapper.Map<Customer>( list );
            
            // Assert

            // There should only be a single customer
            Assert.That( customers.Count() == 1 );

            // There should only be a single Order
            Assert.That( customers.FirstOrDefault().Orders.Count == 1 );

            // There should be two OrderDetails
            Assert.That( customers.FirstOrDefault().Orders.FirstOrDefault().OrderDetails.Count == 2 );
        }

        [Test]
        public void I_Can_Map_Nested_Types_And_Resolve_Duplicate_Entries_Properly_Using_Dynamics()
        {
            // Arrange
            dynamic customer1 = new ExpandoObject();
            customer1.CustomerId = 1;
            customer1.FirstName = "Bob";
            customer1.LastName = "Smith";
            customer1.Orders_OrderId = 1;
            customer1.Orders_OrderTotal = 50.50m;
            customer1.Orders_OrderDetails_OrderDetailId = 1;
            customer1.Orders_OrderDetails_OrderDetailTotal = 25.00m;

            dynamic customer2 = new ExpandoObject();
            customer2.CustomerId = 1;
            customer2.FirstName = "Bob";
            customer2.LastName = "Smith";
            customer2.Orders_OrderId = 1;
            customer2.Orders_OrderTotal = 50.50m;
            customer2.Orders_OrderDetails_OrderDetailId = 2;
            customer2.Orders_OrderDetails_OrderDetailTotal = 25.50m;

            var customerList = new List<dynamic> { customer1, customer2 };

            // Act
            var customers = Slapper.AutoMapper.MapDynamic<Customer>( customerList );

            // Assert

            // There should only be a single customer
            Assert.That( customers.Count() == 1 );

            // There should only be a single Order
            Assert.That( customers.FirstOrDefault().Orders.Count == 1 );

            // There should be two OrderDetails
            Assert.That( customers.FirstOrDefault().Orders.FirstOrDefault().OrderDetails.Count == 2 );
        }

        [Test]
        public void Can_Map_Matching_Field_Names_With_Ease()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>
                                    {
                                        { "Id", 1 },
                                        { "FirstName", "Clark" },
                                        { "LastName", "Kent" }
                                    };

            // Act
            var person = Slapper.AutoMapper.Map<Person>( dictionary );

            // Assert
            Assert.NotNull( person );
            Assert.That( person.Id == 1 );
            Assert.That( person.FirstName == "Clark" );
            Assert.That( person.LastName == "Kent" );
        }

        [Test]
        public void Can_Map_Matching_Field_Names_Using_Dynamic()
        {
            // Arrange
            dynamic dynamicPerson = new ExpandoObject();
            dynamicPerson.Id = 1;
            dynamicPerson.FirstName = "Clark";
            dynamicPerson.LastName = "Kent";

            // Act
            var person = Slapper.AutoMapper.MapDynamic<Person>( dynamicPerson ) as Person;
            
            // Assert
            Assert.NotNull( person );
            Assert.That( person.Id == 1 );
            Assert.That( person.FirstName == "Clark" );
            Assert.That( person.LastName == "Kent" );
        }
    }
}