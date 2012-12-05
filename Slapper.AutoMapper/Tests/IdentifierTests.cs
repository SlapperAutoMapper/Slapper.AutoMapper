using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Slapper.Tests
{
    [TestFixture]
    public class IdentifierTests : TestBase
    {
        public class IdentifierTestModels
        {
            public class Customer
            {
                public int Id;

                public string FirstName;

                public string LastName;
            }

            public class Person
            {
                public int Person_Id;

                public string FirstName;

                public string LastName;
            }

            public class CustomerWithIdAttribute
            {
                [Slapper.AutoMapper.Id]
                public int CustomerId;

                public string FirstName;

                public string LastName;
            }

            public class CustomerWithMultipleIdAttributes
            {
                [Slapper.AutoMapper.Id]
                public int CustomerId;

                [Slapper.AutoMapper.Id]
                public int CustomerType;

                public string FirstName;

                public string LastName;
            }
        }

        [Test]
        public void Can_Add_An_Identifier()
        {
            // Arrange
            const string identifier = "FirstName";

            // Act
            Slapper.AutoMapper.AddIdentifier( typeof( IdentifierTestModels.Customer ), identifier );

            var identifiers = Slapper.AutoMapper.TryToGetIdentifiers( typeof( IdentifierTestModels.Customer ) );

            // Assert
            Assert.That( identifiers.First() == identifier );
        }

        [Test]
        public void Can_Add_Multiple_Identifiers()
        {
            // Arrange
            var identifierList = new List<string> { "FirstName", "LastName" };

            // Act
            Slapper.AutoMapper.AddIdentifiers( typeof( IdentifierTestModels.Customer ), identifierList );

            var identifiers = Slapper.AutoMapper.TryToGetIdentifiers( typeof( IdentifierTestModels.Customer ) );

            // Assert
            foreach ( var identifier in identifierList )
            {
                Assert.That( identifiers.Contains( identifier ) );
            }
        }

        [Test]
        public void Can_Use_Default_Conventions_To_Find_An_Identifier()
        {
            // Act
            var identifiers = Slapper.AutoMapper.TryToGetIdentifiers( typeof( IdentifierTestModels.Customer ) );

            //Assert
            Assert.That( identifiers.First() == "Id" );
        }

        [Test]
        public void Can_Use_A_Custom_Convention_To_Find_An_Identifier()
        {
            // Act
            Slapper.AutoMapper.IdentifierConventions.Add( type => type.Name + "_Id" );

            var identifiers = Slapper.AutoMapper.TryToGetIdentifiers( typeof( IdentifierTestModels.Person ) );

            //Assert
            Assert.That( identifiers.First() == "Person_Id" );
        }

        [Test]
        public void Can_Find_An_Identifier_When_A_Field_Or_Property_Has_An_Id_Attribute()
        {
            // Act
            var identifiers = Slapper.AutoMapper.TryToGetIdentifiers( typeof( IdentifierTestModels.CustomerWithIdAttribute ) );

            //Assert
            Assert.That( identifiers.First() == "CustomerId" );
        }

        [Test]
        public void Can_Find_Identifiers_When_Multiple_Fields_Or_Properties_Have_An_Id_Attribute()
        {
            // Act
            var identifiers = Slapper.AutoMapper.TryToGetIdentifiers( typeof( IdentifierTestModels.CustomerWithMultipleIdAttributes ) );

            //Assert
            Assert.That( identifiers.Contains( "CustomerId" ) && identifiers.Contains( "CustomerType" ) );
        }
    }
}
