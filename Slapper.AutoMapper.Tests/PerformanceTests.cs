using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Slapper.Tests
{
    [TestFixture]
    [Explicit]
    public class PerformanceTests : TestBase
    {
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

        /// <summary>
        /// Simple performance test mapping 50,000 objects.
        /// </summary>
        /// <remarks>
        /// Historical Test Results
        ///     v1.0.0.1: Mapped 50000 objects in 1755 ms.
        ///     v1.0.0.2: Mapped 50000 objects in 1918 ms.
        ///     v1.0.0.3: Mapped 50000 objects in 1819 ms.
        ///     v1.0.0.4: Mapped 50000 objects in 1683 ms.
        ///     v1.0.0.4: Mapped 50000 objects in 1683 ms.
        ///     v1.0.0.5: Mapped 50000 objects in 1877 ms.
        ///     v1.0.0.6: Mapped 50000 objects in 1642 ms.
        ///     v2.0.2  : Mapped 50000 objects in 1171 ms.
        ///     v2.0.3  : Mapped 50000 objects in 472 ms.
        /// </remarks>
        [Test]
        public void Simple_Performance_Test()
        {
            // Arrange
            const int iterations = 50000;

            var list = new List<Dictionary<string, object>>();

            for ( int i = 0; i < iterations; i++ )
            {
                var dictionary = new Dictionary<string, object>
                {
                    { "CustomerId", i },
                    { "FirstName", "Bob" },
                    { "LastName", "Smith" }
                };

                list.Add( dictionary );
            }
            
            // Act
            Stopwatch stopwatch = Stopwatch.StartNew();
            var customers = AutoMapper.Map<Customer>( list );
            stopwatch.Stop();

            // Assert
            Assert.NotNull( customers );
            Assert.That( customers.Count() == iterations );

            Console.WriteLine( string.Format( "Mapped {0} objects in {1} ms.", iterations, stopwatch.ElapsedMilliseconds ) );
        }

        /// <summary>
        /// Complex performance test mapping 50,000 objects with with nested child objects.
        /// </summary>
        /// <remarks>
        /// Historical Test Results
        ///     v1.0.0.1: Mapped 50000 objects in 5913 ms.
        ///     v1.0.0.2: Mapped 50000 objects in 5911 ms.
        ///     v1.0.0.3: Mapped 50000 objects in 5327 ms.
        ///     v1.0.0.4: Mapped 50000 objects in 5349 ms.
        ///     v1.0.0.4: Mapped 50000 objects in 5349 ms.
        ///     v1.0.0.5: Mapped 50000 objects in 5896 ms.
        ///     v1.0.0.6: Mapped 50000 objects in 5539 ms.
        ///     v1.0.0.8: Mapped 50000 objects in 4185 ms.
        ///     v2.0.2  : Mapped 50000 objects in 5348 ms.
        ///     v2.0.3  : Mapped 50000 objects in 3527 ms.
        /// </remarks>
        [Test]
        public void Complex_Performance_Test()
        {
            // Arrange
            const int iterations = 50000;

            var list = new List<Dictionary<string, object>>();

            for ( int i = 0; i < iterations; i++ )
            {
                var dictionary = new Dictionary<string, object>
                {
                    { "CustomerId", i },
                    { "FirstName", "Bob" },
                    { "LastName", "Smith" },
                    { "Orders_OrderId", i },
                    { "Orders_OrderTotal", 50.50m },
                    { "Orders_OrderDetails_OrderDetailId", i },
                    { "Orders_OrderDetails_OrderDetailTotal", 50.50m },
                    { "Orders_OrderDetails_Product_Id", 546 },
                    { "Orders_OrderDetails_Product_ProductName", "Black Bookshelf" }
                };

                list.Add( dictionary );
            }
            
            // Act
            Stopwatch stopwatch = Stopwatch.StartNew();
            var customers = AutoMapper.Map<Customer>( list );
            stopwatch.Stop();

            // Assert
            Assert.NotNull( customers );
            Assert.That( customers.Count() == iterations );

            Console.WriteLine( string.Format( "Mapped {0} objects in {1} ms.", iterations, stopwatch.ElapsedMilliseconds ) );
        }
    }

    [TestFixture]
    [Explicit]
    public class PerformanceTests2 : TestBase
    {
        public enum CustomerStatus
        {
            Active = 1,
            Inactive = 2,
            Suspended = 3,
            Terminated = 4,
            Closed = 5,
            Deleted = 6
        }

        public class Customer
        {
            public int CustomerId { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Suffix { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public CustomerStatus Status { get; set; }
            public int IntProperty { get; set; }
            public int? NullableIntProperty { get; set; }
            public long LongProperty { get; set; }
            public long? NullableLongProperty { get; set; }
            public decimal DecimalProperty { get; set; }
            public Decimal? NullableDecimalProperty { get; set; }
            public Guid GuidProperty { get; set; }
            public Guid? NullableGuidProperty { get; set; }
            public IList<Order> Orders { get; set; }
        }

        public class Order
        {
            public int OrderId { get; set; }
            public decimal OrderTotal { get; set; }
            public IList<OrderDetail> OrderDetails { get; set; }
        }

        public class OrderDetail
        {
            public int OrderDetailId { get; set; }
            public decimal OrderDetailTotal { get; set; }
            public Product Product { get; set; }
        }

        public class Product
        {
            public int Id { get; set; }
            public string ProductName { get; set; }
        }

        /// <summary>
        /// Complex performance test mapping 120,000 objects with multiple nested child objects.
        /// </summary>
        /// <remarks>
        /// Historical Test Results
        ///     v2.0.2: Mapped 120000 objects in 30333 ms.
        ///     v2.0.3: Mapped 120000 objects in 13046 ms.
        /// </remarks>
        [Test]
        public void Complex_Performance_Test()
        {
            // Arrange
            const int iterations = 120000;

            var list = new List<Dictionary<string, object>>();
            var random = new Random();

            for (int i = 0; i < iterations; i++)
            {
                var dictionary = new Dictionary<string, object>
                {
                    { "CustomerId", i },
                    { "FirstName", "Bob" },
                    { "MiddleName", "Lee" },
                    { "LastName", "Smith" },
                    { "Suffix", "Jr" },
                    { "DateOfBirth", "01/01/1980" },
                    { "Status", 1 },
                    { "IntProperty", random.Next(1,10000) },
                    { "NullableIntProperty", random.Next(0,2) == 1 ? null : (int?)random.Next(1,10000) },
                    { "LongProperty", random.Next(1,10000) },
                    { "NullableLongProperty",random.Next(0,2) == 1 ? null : (long?)random.Next(1,10000) },
                    { "DecimalProperty", (decimal?)random.NextDouble() },
                    { "NullableDecimalProperty", random.Next(0,2) == 1 ? null : (decimal?)random.NextDouble() },
                    { "GuidProperty", Guid.NewGuid() },
                    { "NullableGuidProperty", random.Next(0,2) == 1 ? null : (Guid?)Guid.NewGuid() },
                    { "Orders_OrderId", i },
                    { "Orders_OrderTotal", 50.50m },
                    { "Orders_OrderDetails_OrderDetailId", i },
                    { "Orders_OrderDetails_OrderDetailTotal", 50.50m },
                    { "Orders_OrderDetails_Product_Id", random.Next(1,10000) },
                    { "Orders_OrderDetails_Product_ProductName", "Black Bookshelf" }
                };

                list.Add(dictionary);
            }

            // Act
            Stopwatch stopwatch = Stopwatch.StartNew();
            var customers = AutoMapper.Map<Customer>(list).ToList();
            stopwatch.Stop();

            // Assert
            Assert.NotNull(customers);
            Assert.That(customers.Count == iterations);

            Console.WriteLine(string.Format("Mapped {0} objects in {1} ms.", iterations, stopwatch.ElapsedMilliseconds));
        }
    }
}