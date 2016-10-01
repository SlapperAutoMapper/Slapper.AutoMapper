using System.Collections.Generic;
using NUnit.Framework;

namespace Slapper.Tests
{
    [TestFixture]
    public class NullTests : TestBase
    {
        public class PersonWithFields
        {
            public int Id;
            public string FirstName;
            public string LastName;
        }

        public class PersonWithProperties
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        [Test]
        public void Can_Map_Null_Values()
        {
            // Arrange
            const int id = 1;
            const string firstName = null;
            const string lastName = "Smith";

            var dictionary = new Dictionary<string, object>
                {
                    { "Id", id },
                    { "FirstName", null },
                    { "LastName", lastName }
                };

            // Act
            var customer = Slapper.AutoMapper.Map<PersonWithFields>( dictionary );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
        }
    }
}