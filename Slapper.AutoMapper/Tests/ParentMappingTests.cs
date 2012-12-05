using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Slapper.Tests
{
    [TestFixture]
    public class ParentMappingTests : TestBase
    {
        public class Customer
        {
            public int CustomerId;
            public string FirstName;
            public string LastName;
            public IList<Address> Addresses;
        }

        public class Address
        {
            public int AddressId;
            public string Line1;
            public string Line2;
            public string City;
            public string State;
            public string ZipCode;
            public Customer Customer;
        }

        [Test]
        public void Can_Populate_Parent_Objects_Referenced_In_Child_Objects()
        {
            dynamic dynamicCustomer = new ExpandoObject();
            dynamicCustomer.CustomerId = 1;
            dynamicCustomer.FirstName = "Clark";
            dynamicCustomer.LastName = "Kent";
            dynamicCustomer.Addresses_AddressId = 1;
            dynamicCustomer.Addresses_Line1 = "Kent Farm";
            dynamicCustomer.Addresses_Line2 = "Hickory Lane";
            dynamicCustomer.Addresses_City = "Smallville";
            dynamicCustomer.Addresses_State = "Kansas";
            dynamicCustomer.Addresses_ZipCode = "66605";

            dynamic dynamicCustomer2 = new ExpandoObject();
            dynamicCustomer2.CustomerId = 1;
            dynamicCustomer2.FirstName = "Clark";
            dynamicCustomer2.LastName = "Kent";
            dynamicCustomer2.Addresses_AddressId = 2;
            dynamicCustomer2.Addresses_Line1 = "1938 Sullivan Place";
            dynamicCustomer2.Addresses_Line2 = "";
            dynamicCustomer2.Addresses_City = "Metropolis";
            dynamicCustomer2.Addresses_State = "New York ";
            dynamicCustomer2.Addresses_ZipCode = "10012";

            var list = new List<dynamic> { dynamicCustomer, dynamicCustomer2 };

            var customer = Slapper.AutoMapper.MapDynamic<Customer>( list ).FirstOrDefault();

            Assert.NotNull( customer );
            Assert.NotNull( customer.Addresses );
            Assert.NotNull( customer.Addresses.FirstOrDefault().Customer );
            Assert.That( customer.Addresses.FirstOrDefault().Customer == customer );
        }
    }
}
