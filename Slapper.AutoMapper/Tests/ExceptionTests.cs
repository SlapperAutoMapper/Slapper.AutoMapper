using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Slapper.Tests
{
    [TestFixture]
    class ExceptionTests : TestBase
    {
        public class Person
        {
            public int PersonId;
            public string FirstName;
            public string LastName;
        }

        [Test]
        public void Will_Throw_An_Exception_If_The_Type_Is_Not_Dynamic()
        {
            // Arrange
            var someObject = new object();

            // Act
            TestDelegate test = () => Slapper.AutoMapper.MapDynamic<Person>( someObject );

            // Assert
            Assert.Throws<ArgumentException>( test );
        }

        [Test]
        public void Will_Throw_An_Exception_If_The_List_Items_Are_Not_Dynamic()
        {
            // Arrange
            var someObjectList = new List<object> { null };

            // Act
            TestDelegate test = () => Slapper.AutoMapper.MapDynamic<Person>( someObjectList );

            // Assert
            Assert.Throws<ArgumentException>( test );
        }
    }
}
