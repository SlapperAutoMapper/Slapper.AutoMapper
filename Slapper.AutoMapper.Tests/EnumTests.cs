using System.Collections.Generic;
using NUnit.Framework;

namespace Slapper.Tests
{
    [TestFixture]
    public class EnumTests : TestBase
    {
        public enum Gender
        {
            Female = 1,
            Male = 2
        }

        public class PersonWithFields
        {
            public int Id;
            public string FirstName;
            public string LastName;
            public Gender Gender;
        }

        public class PersonWithProperties
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Gender Gender { get; set; }
        }

        [Test]
        public void Can_Map_Enum_Values()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Jimbo";
            const string lastName = "Smith";
            const Gender gender = Gender.Male;

            var dictionary = new Dictionary<string, object>
                {
                    { "Id", id },
                    { "FirstName", firstName },
                    { "LastName", lastName },
                    { "Gender", 2 }
                };

            // Act
            var customer = Slapper.AutoMapper.Map<PersonWithFields>( dictionary );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.Gender == gender );
        }
    }
}