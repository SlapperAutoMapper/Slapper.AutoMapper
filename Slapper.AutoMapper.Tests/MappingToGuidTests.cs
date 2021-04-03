using System;
using System.Dynamic;
using NUnit.Framework;

namespace Slapper.Tests
{
    [TestFixture]
    public class MappingToGuidTests : TestBase
    {
        public class PersonWithFields
        {
            public int Id;
            public string FirstName;
            public string LastName;
            public Guid UniqueId;
            public Guid? ANullableUniqueId;
        }

        public class PersonWithProperties
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Guid UniqueId { get; set; }
            public Guid? ANullableUniqueId { get; set; }
        }

        [Test]
        public void Can_Map_Guid_Values_To_Guid_Fields()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string lastName = "Smith";
            Guid uniqueId = Guid.NewGuid();

            dynamic dynamicPerson = new ExpandoObject();
            dynamicPerson.Id = id;
            dynamicPerson.FirstName = firstName;
            dynamicPerson.LastName = lastName;
            dynamicPerson.UniqueId = uniqueId;

            // Act
            PersonWithFields customer = Slapper.AutoMapper.MapDynamic<PersonWithFields>( dynamicPerson );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.UniqueId == uniqueId );
        }

        [Test]
        public void Can_Map_Guid_Values_To_Guid_Properties()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string lastName = "Smith";
            Guid uniqueId = Guid.NewGuid();

            dynamic dynamicPerson = new ExpandoObject();
            dynamicPerson.Id = id;
            dynamicPerson.FirstName = firstName;
            dynamicPerson.LastName = lastName;
            dynamicPerson.UniqueId = uniqueId;

            // Act
            PersonWithProperties customer = Slapper.AutoMapper.MapDynamic<PersonWithProperties>( dynamicPerson );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.UniqueId == uniqueId );
        }

        [Test]
        public void Can_Map_Guid_String_Values_To_Guid_Fields()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string lastName = "Smith";
            Guid uniqueId = Guid.NewGuid();

            dynamic dynamicPerson = new ExpandoObject();
            dynamicPerson.Id = id;
            dynamicPerson.FirstName = firstName;
            dynamicPerson.LastName = lastName;
            dynamicPerson.UniqueId = uniqueId.ToString(); // This is what we are testing

            // Act
            PersonWithFields customer = Slapper.AutoMapper.MapDynamic<PersonWithFields>( dynamicPerson );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( Equals( customer.UniqueId, uniqueId ) ); // This is what we are testing
        }

        [Test]
        public void Can_Map_Guid_String_Values_To_Guid_Properties()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string lastName = "Smith";
            Guid uniqueId = Guid.NewGuid();

            dynamic dynamicPerson = new ExpandoObject();
            dynamicPerson.Id = id;
            dynamicPerson.FirstName = firstName;
            dynamicPerson.LastName = lastName;
            dynamicPerson.UniqueId = uniqueId.ToString(); // This is what we are testing

            // Act
            PersonWithProperties customer = Slapper.AutoMapper.MapDynamic<PersonWithProperties>( dynamicPerson );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( Equals( customer.UniqueId, uniqueId ) ); // This is what we are testing
        }

        [Test]
        public void Can_Map_Null_Values_To_Guid_Fields()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string lastName = "Smith";
            Guid uniqueId = Guid.NewGuid();
            Guid? aNullableUniqueId = null;

            dynamic dynamicPerson = new ExpandoObject();
            dynamicPerson.Id = id;
            dynamicPerson.FirstName = firstName;
            dynamicPerson.LastName = lastName;
            dynamicPerson.UniqueId = uniqueId.ToString();
            dynamicPerson.ANullableUniqueId = aNullableUniqueId; // This is what we are testing

            // Act
            PersonWithFields customer = Slapper.AutoMapper.MapDynamic<PersonWithFields>( dynamicPerson );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( Equals( customer.UniqueId, uniqueId ) );
            Assert.That( customer.ANullableUniqueId == aNullableUniqueId ); // This is what we are testing
        }

        [Test]
        public void Can_Map_Null_Values_To_Guid_Properties()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string lastName = "Smith";
            Guid uniqueId = Guid.NewGuid();
            Guid? aNullableUniqueId = null;

            dynamic dynamicPerson = new ExpandoObject();
            dynamicPerson.Id = id;
            dynamicPerson.FirstName = firstName;
            dynamicPerson.LastName = lastName;
            dynamicPerson.UniqueId = uniqueId.ToString();
            dynamicPerson.ANullableUniqueId = aNullableUniqueId; // This is what we are testing

            // Act
            PersonWithProperties customer = Slapper.AutoMapper.MapDynamic<PersonWithProperties>( dynamicPerson );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( Equals( customer.UniqueId, uniqueId ) );
            Assert.That( customer.ANullableUniqueId == aNullableUniqueId ); // This is what we are testing
        }

        [Test]
        public void Can_Map_Values_To_Guid_Nullable_Properties()
        {
	        // Arrange
	        const int id = 1;
	        const string firstName = "Bob";
	        const string lastName = "Smith";
	        Guid uniqueId = Guid.NewGuid();
	        Guid? aNullableUniqueId = Guid.NewGuid();

	        dynamic dynamicPerson = new ExpandoObject();
	        dynamicPerson.Id = id;
	        dynamicPerson.FirstName = firstName;
	        dynamicPerson.LastName = lastName;
	        dynamicPerson.UniqueId = uniqueId.ToString();
	        dynamicPerson.ANullableUniqueId = aNullableUniqueId; // This is what we are testing

	        // Act
	        PersonWithProperties customer = Slapper.AutoMapper.MapDynamic<PersonWithProperties>( dynamicPerson );

	        // Assert
	        Assert.NotNull( customer );
	        Assert.That( customer.Id == id );
	        Assert.That( customer.FirstName == firstName );
	        Assert.That( customer.LastName == lastName );
	        Assert.That( Equals( customer.UniqueId, uniqueId ) );
	        Assert.That( customer.ANullableUniqueId == aNullableUniqueId ); // This is what we are testing
        }
    }
}