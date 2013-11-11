using System.Collections.Generic;
using System.Dynamic;
using NUnit.Framework;

namespace Slapper.Tests
{
    [TestFixture]
    public class MappingToEnumTests : TestBase
    {
        public enum Gender
        {
            Female = 1,
            Male = 2
        }

        public enum MaritalStatus
        {
            Married = 1,
            Single = 2
        }

        public class PersonWithFields
        {
            public int Id;
            public string FirstName;
            public string LastName;
            public Gender Gender;
            public MaritalStatus? MaritalStatus;
        }

        public class PersonWithProperties
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Gender Gender { get; set; }
            public MaritalStatus? MaritalStatus { get; set; }
        }

        [Test]
        public void Can_Map_Enum_Values_To_Enum_Fields()
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
                    { "Gender", gender }
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

        [Test]
        public void Can_Map_Enum_Values_To_Enum_Properties()
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
                    { "Gender", gender }
                };

            // Act
            var customer = Slapper.AutoMapper.Map<PersonWithProperties>( dictionary );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.Gender == gender );
        }

        [Test]
        public void Can_Map_Integer_Values_To_Enum_Fields()
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

        [Test]
        public void Can_Map_Integer_Values_To_Enum_Properties()
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
            var customer = Slapper.AutoMapper.Map<PersonWithProperties>( dictionary );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.Gender == gender );
        }

        [Test]
        public void Can_Map_String_Values_To_Enum_Fields()
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
                    { "Gender", "2" }
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

        [Test]
        public void Can_Map_String_Values_To_Enum_Properties()
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
                    { "Gender", "2" }
                };

            // Act
            var customer = Slapper.AutoMapper.Map<PersonWithProperties>( dictionary );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.Gender == gender );
        }

        [Test]
        public void Can_Map_Null_Values_To_Nullable_Enum_Fields()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Jimbo";
            const string lastName = "Smith";
            const Gender gender = Gender.Male;
            MaritalStatus? maritalStatus = null;

            dynamic person = new ExpandoObject();
            person.Id = id;
            person.FirstName = firstName;
            person.LastName = lastName;
            person.Gender = gender;
            person.MaritalStatus = maritalStatus;

            // Act
            PersonWithFields customer = Slapper.AutoMapper.MapDynamic<PersonWithFields>( person );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.Gender == gender );
            Assert.That( customer.MaritalStatus == maritalStatus );
        }

        [Test]
        public void Can_Map_Null_Values_To_Nullable_Enum_Properties()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Jimbo";
            const string lastName = "Smith";
            const Gender gender = Gender.Male;
            MaritalStatus? maritalStatus = null;

            dynamic person = new ExpandoObject();
            person.Id = id;
            person.FirstName = firstName;
            person.LastName = lastName;
            person.Gender = gender;
            person.MaritalStatus = maritalStatus;

            // Act
            PersonWithProperties customer = Slapper.AutoMapper.MapDynamic<PersonWithProperties>( person );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.Gender == gender );
            Assert.That( customer.MaritalStatus == maritalStatus );
        }
    }
}