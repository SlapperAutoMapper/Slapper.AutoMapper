using System.Collections.Generic;
using NUnit.Framework;

namespace Slapper.Tests
{
    [TestFixture]
    public class ArrayTests : TestBase
    {
        public class PersonWithFields
        {
            public int Id;
            public string FirstName;
            public string LastName;
            public string[] FavoriteFoods;
        }

        public class PersonWithProperties
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string[] FavoriteFoods { get; set; }
        }

        [Test]
        public void Can_Map_Null_Values_To_Null_Arrays()
        {
            // Arrange
            const int id = 1;
            const string firstName = null;
            const string lastName = "Smith";
            const string[] favoriteFoods = null;

            var dictionary = new Dictionary<string, object>
                {
                    { "Id", id },
                    { "FirstName", null },
                    { "LastName", lastName },
                    { "FavoriteFoods", favoriteFoods }
                };

            // Act
            var customer = Slapper.AutoMapper.Map<PersonWithFields>( dictionary );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.Null( customer.FavoriteFoods );
        }

        [Test]
        public void Can_Map_Array_Values_To_Arrays()
        {
            // Arrange
            const int id = 1;
            const string firstName = null;
            const string lastName = "Smith";
            string[] favoriteFoods = new [] { "Ice Cream", "Jello" };

            var dictionary = new Dictionary<string, object>
                {
                    { "Id", id },
                    { "FirstName", null },
                    { "LastName", lastName },
                    { "FavoriteFoods", favoriteFoods }
                };

            // Act
            var customer = Slapper.AutoMapper.Map<PersonWithFields>( dictionary );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.FavoriteFoods == favoriteFoods );
        }
    }
}