using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NUnit.Framework;

namespace Slapper.Tests
{
    [TestFixture]
    public class MapDynamicTests : TestBase
    {
        public class Customer
        {
            public int Id;
            public string FirstName;
            public string LastName;
            public ICollection<Order> Orders;
        }

        public class Order
        {
            public int Id;
            public decimal OrderTotal;
        }

        public class CustomerSingleOrder
        {
            public int Id;
            public string FirstName;
            public string LastName;
            public SingleOrder Order;
        }

        public class SingleOrder
        {
            public int Id;
            public OrderDetails Details;
        }

        public class OrderDetails
        {
            public string Address;
        }

        [Test]
        public void Can_Handle_Mapping_A_Single_Dynamic_Object()
        {
            // Arrange
            dynamic dynamicCustomer = new ExpandoObject();
            dynamicCustomer.Id = 1;
            dynamicCustomer.FirstName = "Bob";
            dynamicCustomer.LastName = "Smith";
            dynamicCustomer.Orders_Id = 1;
            dynamicCustomer.Orders_OrderTotal = 50.50m;

            // Act
            var customer = Slapper.AutoMapper.MapDynamic<Customer>( dynamicCustomer ) as Customer;

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Orders.Count == 1 );
        }

        [Test]
        public void Can_Handle_Mapping_Nested_Members_Using_Dynamic()
        {
            // Arrange
            var dynamicCustomers = new List<object>();

            for ( int i = 0; i < 5; i++ )
            {
                dynamic customer = new ExpandoObject();
                customer.Id = i;
                customer.FirstName = "FirstName" + i;
                customer.LastName = "LastName" + i;
                customer.Orders_Id = i;
                customer.Orders_OrderTotal = i + 0m;

                dynamicCustomers.Add( customer );
            }

            // Act
            var customers = Slapper.AutoMapper.MapDynamic<Customer>( dynamicCustomers );

            // Assert
            Assert.That( customers.Count() == 5 );
            Assert.That( customers.First().Orders.Count == 1 );
        }

        [Test]
        public void Nested_Member_Should_Be_Null_If_All_Values_Are_Null()
        {
            // Arrange
            dynamic customer = new ExpandoObject();
            customer.Id = 1;
            customer.FirstName = "FirstName";
            customer.LastName = "LastName";
            customer.Order_Id = null;
            customer.Order_OrderTotal = null;

            // Act
            var test = Slapper.AutoMapper.MapDynamic<CustomerSingleOrder>(customer);

            // Assert
            Assert.That(test != null);
            Assert.That(test.Order == null);
        }

        [Test]
        public void Nested_Member_Should_Be_Null_Only_If_All_Nested_Values_Are_Null()
        {
            // Arrange
            dynamic customer = new ExpandoObject();
            customer.Id = 1;
            customer.FirstName = "FirstName";
            customer.LastName = "LastName";
            customer.Order_Id = null;
            customer.Order_OrderTotal = null;
            customer.Order_Details_Address = "123 Fake Ave.";

            // Act
            var test = Slapper.AutoMapper.MapDynamic<CustomerSingleOrder>(customer);

            // Assert
            Assert.That(test != null);
            Assert.That(test.Order != null);
            Assert.That(test.Order.Details != null);
            Assert.That(!string.IsNullOrWhiteSpace(test.Order.Details.Address));
            Assert.That(test.Order.Details.Address == "123 Fake Ave.");
        }
    }
}