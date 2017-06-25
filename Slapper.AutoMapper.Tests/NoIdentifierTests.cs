using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NUnit.Framework;

namespace Slapper.Tests
{
    [TestFixture]
    public class NoIdentifierTests : TestBase
    {
        public class PersonWithFields
        {
            public string FirstName;
            public string LastName;
        }

        [Test]
        public void Can_Map_To_Types_With_No_Identifiers()
        {
            // Arrange
            const string person1FirstName = "Bob";
            const string person1LastName = "Smith";
            const string person2FirstName = "Nancy";
            const string person2LastName = "Sue";

            dynamic person1 = new ExpandoObject();
            person1.FirstName = person1FirstName;
            person1.LastName = person1LastName;

            dynamic person2 = new ExpandoObject();
            person2.FirstName = person2FirstName;
            person2.LastName = person2LastName;

            var list = new List<dynamic> { person1, person2 };

            // Act
            var persons = Slapper.AutoMapper.MapDynamic<PersonWithFields>( list ).ToList();
            
            // Assert
            Assert.NotNull( persons );
            Assert.That( persons.Count == 2 );
            Assert.That( persons[ 0 ].FirstName == person1FirstName );
            Assert.That( persons[ 0 ].LastName == person1LastName );
            Assert.That( persons[ 1 ].FirstName == person2FirstName );
            Assert.That( persons[ 1 ].LastName == person2LastName );
        }
    }
}