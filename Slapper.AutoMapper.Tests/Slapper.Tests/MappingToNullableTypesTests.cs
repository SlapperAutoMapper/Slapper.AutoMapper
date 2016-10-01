using System;
using System.Dynamic;
using NUnit.Framework;

namespace Slapper.Tests
{
    [TestFixture]
    public class MappingToDateTimeTests : TestBase
    {
        public class PersonWithFields
        {
            public int Id;
            public string FirstName;
            public string LastName;
            public DateTime StartDate;
            public DateTime? EndDate;
        }

        public class PersonWithProperties
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        [Test]
        public void Can_Map_DateTime_Values_To_Nullable_DateTime_Fields()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string lastName = "Smith";
            DateTime startDate = DateTime.Now.AddDays( - 2 );
            DateTime endDate = DateTime.Now;

            dynamic dynamicPerson = new ExpandoObject();
            dynamicPerson.Id = id;
            dynamicPerson.FirstName = firstName;
            dynamicPerson.LastName = lastName;
            dynamicPerson.StartDate = startDate;
            dynamicPerson.EndDate = endDate;

            // Act
            PersonWithFields customer = Slapper.AutoMapper.MapDynamic<PersonWithFields>( dynamicPerson );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.StartDate == startDate );
            Assert.That( customer.EndDate == endDate );
        }

        [Test]
        public void Can_Map_DateTime_Values_To_Nullable_DateTime_Properties()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string lastName = "Smith";
            DateTime startDate = DateTime.Now.AddDays( -2 );
            DateTime endDate = DateTime.Now; // This is what we are testing

            dynamic dynamicPerson = new ExpandoObject();
            dynamicPerson.Id = id;
            dynamicPerson.FirstName = firstName;
            dynamicPerson.LastName = lastName;
            dynamicPerson.StartDate = startDate;
            dynamicPerson.EndDate = endDate;

            // Act
            PersonWithProperties customer = Slapper.AutoMapper.MapDynamic<PersonWithProperties>( dynamicPerson );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.StartDate == startDate );
            Assert.That( customer.EndDate == endDate ); // This is what we are testing
        }

        [Test]
        public void Can_Map_Null_Values_To_Nullable_DateTime_Fields()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string lastName = "Smith";
            DateTime startDate = DateTime.Now.AddDays( -2 );
            DateTime? endDate = null; // This is what we are testing

            dynamic dynamicPerson = new ExpandoObject();
            dynamicPerson.Id = id;
            dynamicPerson.FirstName = firstName;
            dynamicPerson.LastName = lastName;
            dynamicPerson.StartDate = startDate;
            dynamicPerson.EndDate = endDate;

            // Act
            PersonWithFields customer = Slapper.AutoMapper.MapDynamic<PersonWithFields>( dynamicPerson );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.StartDate == startDate );
            Assert.That( customer.EndDate == endDate ); // This is what we are testing
        }

        [Test]
        public void Can_Map_Null_Values_To_Nullable_DateTime_Properties()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string lastName = "Smith";
            DateTime startDate = DateTime.Now.AddDays( -2 );
            DateTime? endDate = null; // This is what we are testing

            dynamic dynamicPerson = new ExpandoObject();
            dynamicPerson.Id = id;
            dynamicPerson.FirstName = firstName;
            dynamicPerson.LastName = lastName;
            dynamicPerson.StartDate = startDate;
            dynamicPerson.EndDate = endDate;

            // Act
            PersonWithProperties customer = Slapper.AutoMapper.MapDynamic<PersonWithProperties>( dynamicPerson );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.StartDate == startDate );
            Assert.That( customer.EndDate == endDate ); // This is what we are testing
        }

        [Test]
        public void Can_Map_DateTime_String_Values_To_Nullable_DateTime_Fields()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string lastName = "Smith";
            DateTime startDate = DateTime.Now.AddDays( -2 );
            DateTime? endDate = DateTime.Now;

            dynamic dynamicPerson = new ExpandoObject();
            dynamicPerson.Id = id;
            dynamicPerson.FirstName = firstName;
            dynamicPerson.LastName = lastName;
            dynamicPerson.StartDate = startDate.ToString();
            dynamicPerson.EndDate = endDate.ToString();

            // Act
            PersonWithFields customer = Slapper.AutoMapper.MapDynamic<PersonWithFields>( dynamicPerson );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.StartDate.ToString() == startDate.ToString() );
            Assert.That( customer.EndDate.ToString() == endDate.ToString() );
        }

        [Test]
        public void Can_Map_DateTime_String_Values_To_Nullable_DateTime_Properties()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string lastName = "Smith";
            DateTime startDate = DateTime.Now.AddDays( -2 );
            DateTime? endDate = DateTime.Now;

            dynamic dynamicPerson = new ExpandoObject();
            dynamicPerson.Id = id;
            dynamicPerson.FirstName = firstName;
            dynamicPerson.LastName = lastName;
            dynamicPerson.StartDate = startDate.ToString();
            dynamicPerson.EndDate = endDate.ToString();

            // Act
            PersonWithProperties customer = Slapper.AutoMapper.MapDynamic<PersonWithProperties>( dynamicPerson );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.StartDate.ToString() == startDate.ToString() );
            Assert.That( customer.EndDate.ToString() == endDate.ToString() );
        }
    }
}