using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Slapper.Tests
{
    [TestFixture]
    public class SafeThereadTest: TestBase
    {
        public SafeThereadTest()
        {
            AutoMapper.Configuration.AddIdentifiers(typeof(NameValue), new List<string> { "Id", "Name" });
        }

        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public IEnumerable<NameValue> Phones { get; set; }
            public IEnumerable<NameValue> Emails { get; set; }
        }

        public class NameValue
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [Test]
        public void Can_Map_Diferents_Object_ThreadSafe()
        {
            dynamic dynamicCustomer = new ExpandoObject();
            dynamicCustomer.Id = 1;
            dynamicCustomer.Name = "Clark";
            dynamicCustomer.Phones_Id = 1;
            dynamicCustomer.Phones_Name = "88888";
            dynamicCustomer.Emails_Id = 1;
            dynamicCustomer.Emails_Name = "a@b.com";

            dynamic dynamicCustomer2 = new ExpandoObject();
            dynamicCustomer2.Id = 1;
            dynamicCustomer2.Name = "Clark";
            dynamicCustomer2.Phones_Id = 2;
            dynamicCustomer2.Phones_Name = "99999";
            dynamicCustomer2.Emails_Id = 2;
            dynamicCustomer2.Emails_Name = "c@c.com";

            dynamic dynamicCustomer3 = new ExpandoObject();
            dynamicCustomer3.Id = 1;
            dynamicCustomer3.Name = "Clark";
            dynamicCustomer3.Phones_Id = 1;
            dynamicCustomer3.Phones_Name = "88888";
            dynamicCustomer3.Emails_Id = 1;
            dynamicCustomer3.Emails_Name = "a@b.com";

            dynamic dynamicCustomer4 = new ExpandoObject();
            dynamicCustomer4.Id = 1;
            dynamicCustomer4.Name = "Clark";
            dynamicCustomer4.Phones_Id = 2;
            dynamicCustomer4.Phones_Name = "99999";
            dynamicCustomer4.Emails_Id = 2;
            dynamicCustomer4.Emails_Name = "c@c.com";

            var task1 = Task.Factory.StartNew(() =>
            {
                var list = new List<dynamic> { dynamicCustomer, dynamicCustomer2 };
                return AutoMapper.MapDynamic<Customer>(list).FirstOrDefault();
            });

            var task2 = Task.Factory.StartNew(() => 
            {
                var list = new List<dynamic> { dynamicCustomer3, dynamicCustomer4 };
                return AutoMapper.MapDynamic<Customer>(list).FirstOrDefault();
            });

            Task.WaitAll(task1, task2);

            var customer = task1.Result;
            Assert.NotNull(customer);
            Assert.AreEqual("Clark", customer.Name);

            Assert.AreEqual("a@b.com", customer.Emails.Single(em => em.Id == 1).Name);
            Assert.AreEqual("c@c.com", customer.Emails.Single(em => em.Id == 2).Name);

            Assert.AreEqual("88888", customer.Phones.Single(ph => ph.Id == 1).Name);
            Assert.AreEqual("99999", customer.Phones.Single(ph => ph.Id == 2).Name);

            var customer2 = task2.Result;
            Assert.NotNull(customer2);
            Assert.AreEqual("Clark", customer2.Name);

            Assert.AreEqual("a@b.com", customer2.Emails.Single(em => em.Id == 1).Name);
            Assert.AreEqual("c@c.com", customer2.Emails.Single(em => em.Id == 2).Name);

            Assert.AreEqual("88888", customer2.Phones.Single(ph => ph.Id == 1).Name);
            Assert.AreEqual("99999", customer2.Phones.Single(ph => ph.Id == 2).Name);
        }

        [Test]
        public void Can_Map_Diferents_Objects_With_Same_IDs_And_KeepCache_False_To_Avoid_Conflicts_ThreadSafe()
        {
            dynamic dynamicCustomer = new ExpandoObject();
            dynamicCustomer.Id = 1;
            dynamicCustomer.Name = "Clark";
            dynamicCustomer.Phones_Id = 1;
            dynamicCustomer.Phones_Name = "88888";
            dynamicCustomer.Emails_Id = 1;
            dynamicCustomer.Emails_Name = "a@b.com";

            dynamic dynamicCustomer2 = new ExpandoObject();
            dynamicCustomer2.Id = 1;
            dynamicCustomer2.Name = "Clark";
            dynamicCustomer2.Phones_Id = 2;
            dynamicCustomer2.Phones_Name = "99999";
            dynamicCustomer2.Emails_Id = 2;
            dynamicCustomer2.Emails_Name = "c@c.com";

            dynamic dynamicCustomer3 = new ExpandoObject();
            dynamicCustomer3.Id = 1;
            dynamicCustomer3.Name = "Mark";
            dynamicCustomer3.Phones_Id = 1;
            dynamicCustomer3.Phones_Name = "77777";
            dynamicCustomer3.Emails_Id = 1;
            dynamicCustomer3.Emails_Name = "aa@bb.com";

            dynamic dynamicCustomer4 = new ExpandoObject();
            dynamicCustomer4.Id = 1;
            dynamicCustomer4.Name = "Mark";
            dynamicCustomer4.Phones_Id = 2;
            dynamicCustomer4.Phones_Name = "66666";
            dynamicCustomer4.Emails_Id = 2;
            dynamicCustomer4.Emails_Name = "cc@cc.com";

            var task1 = Task.Factory.StartNew(() =>
            {
                var list = new List<dynamic> { dynamicCustomer, dynamicCustomer2 };
                return AutoMapper.MapDynamic<Customer>(list, false).FirstOrDefault();
            });

            var task2 = Task.Factory.StartNew(() =>
            {
                var list = new List<dynamic> { dynamicCustomer3, dynamicCustomer4 };
                return AutoMapper.MapDynamic<Customer>(list, false).FirstOrDefault();
            });

            Task.WaitAll(task1, task2);

            var customer = task1.Result;
            Assert.NotNull(customer);
            Assert.AreEqual("Clark", customer.Name);

            Assert.AreEqual("a@b.com", customer.Emails.Single(em => em.Id == 1).Name);
            Assert.AreEqual("c@c.com", customer.Emails.Single(em => em.Id == 2).Name);

            Assert.AreEqual("88888", customer.Phones.Single(ph => ph.Id == 1).Name);
            Assert.AreEqual("99999", customer.Phones.Single(ph => ph.Id == 2).Name);

            var customer2 = task2.Result;
            Assert.NotNull(customer2);
            Assert.AreEqual("Mark", customer2.Name);

            Assert.AreEqual("aa@bb.com", customer2.Emails.Single(em => em.Id == 1).Name);
            Assert.AreEqual("cc@cc.com", customer2.Emails.Single(em => em.Id == 2).Name);

            Assert.AreEqual("77777", customer2.Phones.Single(ph => ph.Id == 1).Name);
            Assert.AreEqual("66666", customer2.Phones.Single(ph => ph.Id == 2).Name);
        }
    }
}
