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


        public class OrderWithLongId
        {
            public long Id { get; set; }
            public List<OrderItem> OrderItems { get; set; }
        }

        [Test]
        public void Previously_Instantiated_Objects_Will_Be_Returned_Only_When_The_Same_InstanceCache_Is_Used()
        {
            var cache = new Slapper.AutoMapper.InstanceCache();
            var first = Slapper.AutoMapper.Map<Customer>(new[] { new Dictionary<string, object> { { "CustomerId", 1 }, { "FirstName", "Bob" } } }, cache).Single();
            var second = Slapper.AutoMapper.Map<Customer>(new[] { new Dictionary<string, object> { { "CustomerId", 1 } } }, cache).Single();
            Assert.AreSame(first, second);
            Assert.AreEqual("Bob", second.FirstName);

            cache.Clear();
            var third = Slapper.AutoMapper.Map<Customer>(new[] { new Dictionary<string, object> { { "CustomerId", 1 } } }, cache).Single();
            Assert.AreNotSame(first, third);
            Assert.Null(third.FirstName);
        }

        [Test]
        public void Separate_Calls_Do_Not_Share_Instances()
        {
            var customer = Slapper.AutoMapper.Map<Customer>(new Dictionary<string, object> { { "CustomerId", 1 }, { "FirstName", "Bob" } });
            var customer2 = Slapper.AutoMapper.Map<Customer>(new Dictionary<string, object> { { "CustomerId", 1 } });

            Assert.AreNotSame(customer, customer2);
            Assert.AreEqual("Bob", customer.FirstName);
            Assert.Null(customer2.FirstName);
        }

        [Test]
        public void Concurrent_Mappings_Produce_Correct_Results()
        {
            var bad = 0;
            for (var i = 0; i < 50; i++)
            {
                var results = new Order[16][];
                System.Threading.Tasks.Parallel.For(0, 16, k =>
                {
                    var rows = new List<IDictionary<string, object>>();
                    for (var p = 1; p <= 50; p++)
                        for (var c = 1; c <= 3; c++)
                            rows.Add(new Dictionary<string, object> { { "Id", p }, { "OrderItems_Id", p * 1000 + c } });
                    try { results[k] = Slapper.AutoMapper.Map<Order>(rows).ToArray(); }
                    catch { results[k] = null; }
                });
                bad += results.Count(r => r == null || r.Length != 50 || r.Any(o => o.OrderItems?.Count != 3));
            }
            Assert.AreEqual(0, bad);
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
        public void Test_Long_Ids_With_Colliding_HashValues()
        {
            // This test could fail if MS GetHashCode implementation for long changed. We would then have to find new long values
            // having the same hashcode.
            const long longId1 = 95988224123597;
            var item1 = new Dictionary<string, object>()
                                               {
                                                   { "Id", longId1 },
                                                   { "OrderDetail_Id", 1 }
                                               };

            const long longId2 = 95983929156300;
            var item2 = new Dictionary<string, object>()
                                               {
                                                   { "Id", longId2 },
                                                   { "OrderDetail_Id", 2 }
                                               };

            var list = new List<Dictionary<string, object>>() { item1, item2 };
            var orderList = AutoMapper.Map<OrderWithLongId>(list).ToList();

            Assert.AreEqual(longId1.GetHashCode(), longId2.GetHashCode());
            Assert.AreEqual(orderList.Count, list.Count);
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