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

            var customers = Slapper.AutoMapper.Map<Customer>( list );

            // Assert
            Assert.NotNull( customers );
            Assert.That( customers.Count() == iterations );

            Debug.WriteLine( "Mapped {0} objects in {1} ms.", iterations, stopwatch.ElapsedMilliseconds );
        }

        /// <summary>
        /// Complex performance test mapping 50,000 objects with with nested child objects.
        /// </summary>
        /// <remarks>
        /// Historical Test Results
        ///     v1.0.0.1: Mapped 50000 objects in 5913 ms.
        ///     v1.0.0.2: Mapped 50000 objects in 5911 ms.
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
                    { "Orders_Id", i },
                    { "Orders_OrderTotal", 50.50m },
                    { "Orders_OrderDetails_Id", i },
                    { "Orders_OrderDetails_OrderDetailTotal", 50.50m },
                    { "Orders_OrderDetails_Product_Id", 546 },
                    { "Orders_OrderDetails_Product_ProductName", "Black Bookshelf" }
                };

                list.Add( dictionary );
            }
            
            // Act
            Stopwatch stopwatch = Stopwatch.StartNew();

            var customers = Slapper.AutoMapper.Map<Customer>( list );

            // Assert
            Assert.NotNull( customers );
            Assert.That( customers.Count() == iterations );

            Debug.WriteLine( "Mapped {0} objects in {1} ms.", iterations, stopwatch.ElapsedMilliseconds );
        }
    }
}
