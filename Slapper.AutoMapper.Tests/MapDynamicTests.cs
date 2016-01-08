﻿using System;
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

        [Test]
        public void Returns_Empty_List_For_Null_Dictionary()
        {
            // Arrange

            // Act
            var result = Slapper.AutoMapper.MapDynamic<Customer>(null);

            // Assert
            Assert.NotNull(result);
            Assert.That(!result.Any());
        }

        [Test]
        public void Returns_Null_For_Reference_Types()
        {
            // Arrange
            object nullDictionary = null;

            // Act
            var result = Slapper.AutoMapper.MapDynamic<Customer>(nullDictionary);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Returns_Zero_For_Numeric_Value_Types()
        {
            // Arrange
            object nullDictionary = null;

            // Act
            var result = Slapper.AutoMapper.MapDynamic<int>(nullDictionary);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result == 0);
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
    }
}