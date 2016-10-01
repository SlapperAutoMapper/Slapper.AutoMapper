using System.Collections.Generic;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Slapper.Tests
{
    using System;
    using System.Linq;

    [TestFixture]
    public class CachingBehaviorTests : TestBase
    {
        public class Customer
        {
            public int CustomerId;

            public string FirstName;

            public string LastName;
        }

        public class Employee
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public Department Department { get; set; }
        }

        public class Department
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        public class Order
        {
            public int Id { get; set; }
            public List<OrderItem> OrderItems { get; set; }
        }

        public class OrderItem
        {
            public int Id { get; set; }
        }

        [Test]
        public void Previously_Instantiated_Objects_Will_Be_Returned_Until_The_Cache_Is_Cleared()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>
                             {
                                 { "CustomerId", 1 },
                                 { "FirstName", "Bob" },
                                 { "LastName", "Smith" }
                             };

            // Act
            var customer = Slapper.AutoMapper.Map<Customer>(dictionary);

            // Assert
            Assert.AreEqual("Bob", customer.FirstName);

            // Arrange
            var dictionary2 = new Dictionary<string, object> { { "CustomerId", 1 } };

            // Act
            var customer2 = Slapper.AutoMapper.Map<Customer>(dictionary2);

            // Assert that this will be "Bob" because the identifier of the Customer object was the same, 
            // so we recieved back the cached instance of the Customer object.
            Assert.AreEqual("Bob", customer2.FirstName);

            // Arrange
            var dictionary3 = new Dictionary<string, object> { { "CustomerId", 1 } };

            Slapper.AutoMapper.Cache.ClearInstanceCache();

            // Act
            var customer3 = Slapper.AutoMapper.Map<Customer>(dictionary3);

            // Assert
            Assert.Null(customer3.FirstName);
        }

        [Test]
        public void Test_Nested_Duplicate_Instances()
        {
            var item1 = new Dictionary<string, object>()
                                               {
                                                   { "Id", 1 },
                                                   { "Name", "Employee1" },
                                                   { "Department_Id", 1 },
                                                   { "Department_Name", "Department1" }
                                               };

            var item2 = new Dictionary<string, object>()
                                               {
                                                   { "Id", 2 },
                                                   { "Name", "Employee2" },
                                                   { "Department_Id", 1 },
                                                   { "Department_Name", "Department1" }
                                               };

            var list = new List<Dictionary<string, object>>() { item1, item2 };
            var employeeList = AutoMapper.Map<Employee>(list).ToList();

            Assert.AreSame(employeeList[0].Department, employeeList[1].Department);           
        }

        [Test]
        public void Cache_is_cleared_if_KeepCache_is_false()
        {
            var item1 = new Dictionary<string, object> {
                { "Id", 1 },
                { "OrderItems_Id", 1 }
            };

            var item2 = new Dictionary<string, object> {
                { "Id", 1 },
                { "OrderItems_Id", 2 }
            };

            var firstResult = AutoMapper.Map<Order>(item1, false);
            var secondResult = AutoMapper.Map<Order>(item2, false);

            Assert.AreEqual(1, firstResult.OrderItems.Count);
            Assert.AreEqual(1, firstResult.OrderItems[0].Id);
            Assert.AreEqual(1, secondResult.OrderItems.Count);
            Assert.AreEqual(2, secondResult.OrderItems[0].Id);
        }
    }
}