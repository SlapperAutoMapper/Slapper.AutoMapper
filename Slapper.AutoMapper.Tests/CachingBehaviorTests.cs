using System.Collections.Generic;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Slapper.Tests
{
    [TestFixture]
    public class CachingBehaviorTests : TestBase
    {
        public class Customer
        {
            public int CustomerId;
            public string FirstName;
            public string LastName;
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
            var customer = Slapper.AutoMapper.Map<Customer>( dictionary );

            // Assert
            Assert.That( customer.FirstName == "Bob" );

            // Arrange
            var dictionary2 = new Dictionary<string, object>
                                  {
                                      { "CustomerId", 1 }
                                  };

            // Act
            var customer2 = Slapper.AutoMapper.Map<Customer>( dictionary2 );

            // Assert that this will be "Bob" because the identifier of the Customer object was the same, 
            // so we recieved back the cached instance of the Customer object.
            Assert.That( customer2.FirstName == "Bob" );

            // Arrange
            var dictionary3 = new Dictionary<string, object>
                                  {
                                      { "CustomerId", 1 }
                                  };

            Slapper.AutoMapper.Cache.ClearInstanceCache();

            // Act
            var customer3 = Slapper.AutoMapper.Map<Customer>( dictionary3 );

            // Assert
            Assert.That( customer3.FirstName == null );
        }
    }
}