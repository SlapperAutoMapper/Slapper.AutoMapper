using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Slapper.Tests
{
    [TestFixture]
    public class MappingWithPropertyAttributesTests : TestBase
    {
        public class Person
        {
            [Slapper.AutoMapper.ColumnName("PersonId")]
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        [Test]
        public void Can_Add_An_ColumnId()
        {
            // Arrange
            dynamic dynamicPerson = new ExpandoObject();
            dynamicPerson.PersonId = 1;
            dynamicPerson.FirstName = "Bob";
            dynamicPerson.LastName = "Smith";

            // Act
            Slapper.AutoMapper.Configuration.AddIdentifier(typeof(Person), "PersonId");
            var person = Slapper.AutoMapper.MapDynamic<Person>(dynamicPerson) as Person;


            // Assert
            Assert.NotNull(person);
        }
    }
}
