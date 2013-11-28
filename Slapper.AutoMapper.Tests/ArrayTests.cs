using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Slapper.Tests
{
    [TestFixture]
    public class ArrayTests : TestBase
    {
        public class PersonWithFields
        {
            public int Id;
            public string FirstName;
            public string LastName;
            public string[] FavoriteFoods;
        }

        public class PersonWithProperties
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string[] FavoriteFoods { get; set; }
        }

        public class Department
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public PersonWithProperties[] Staff { get; set; }
        }

        [Test]
        public void Can_Map_Null_Values_To_Null_Arrays()
        {
            // Arrange
            const int id = 1;
            const string firstName = null;
            const string lastName = "Smith";
            const string[] favoriteFoods = null;

            var dictionary = new Dictionary<string, object>
                {
                    { "Id", id },
                    { "FirstName", null },
                    { "LastName", lastName },
                    { "FavoriteFoods", favoriteFoods }
                };

            // Act
            var customer = Slapper.AutoMapper.Map<PersonWithFields>( dictionary );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.Null( customer.FavoriteFoods );
        }

        [Test]
        public void Can_Map_Array_Values_To_Arrays()
        {
            // Arrange
            const int id = 1;
            const string firstName = null;
            const string lastName = "Smith";
            string[] favoriteFoods = new [] { "Ice Cream", "Jello" };

            var dictionary = new Dictionary<string, object>
                {
                    { "Id", id },
                    { "FirstName", null },
                    { "LastName", lastName },
                    { "FavoriteFoods", favoriteFoods }
                };

            // Act
            var customer = Slapper.AutoMapper.Map<PersonWithFields>( dictionary );

            // Assert
            Assert.NotNull( customer );
            Assert.That( customer.Id == id );
            Assert.That( customer.FirstName == firstName );
            Assert.That( customer.LastName == lastName );
            Assert.That( customer.FavoriteFoods == favoriteFoods );
        }

        [Test]
        public void Can_Map_Multiple_Dictionaries_To_Arrays()
        {
            // Arrange
            const int id = 1;
            const string name = "IT";
            const int firstEmployeeId = 1;
            const string firstEmployeeLastName = "Smith";
            const int secondEmployeeId = 2;
            const string secondEmployeeLastName = "Doe";

            var firstDictionary = new Dictionary<string, object>
            {
                {"Id", id},
                {"Name", name},
                {"Staff_Id", firstEmployeeId},
                {"Staff_LastName", firstEmployeeLastName}
            };

            var secondDictionary = new Dictionary<string, object>
            {
                {"Id", id},
                {"Name", name},
                {"Staff_Id", secondEmployeeId},
                {"Staff_LastName", secondEmployeeLastName}
            };

            // Act
            var department =
                AutoMapper.Map<Department>(new List<IDictionary<string, object>> {firstDictionary, secondDictionary}).Single();

            // Assert
            Assert.That(department.Id == id);
            Assert.That(department.Name == name);
            Assert.That(department.Staff.Count() == 2);
            Assert.That(department.Staff[0].Id == firstEmployeeId);
            Assert.That(department.Staff[0].LastName == firstEmployeeLastName);
            Assert.That(department.Staff[1].Id == secondEmployeeId);
            Assert.That(department.Staff[1].LastName == secondEmployeeLastName);
        }

        public class Customer
        {
            public int CustomerId;
            public string FirstName;
            public string LastName;
            public Order[] Orders;
        }

        public class Order
        {
            public int OrderId;
            public decimal OrderTotal;
            public OrderDetail[] OrderDetails;
        }

        public class OrderDetail
        {
            public int OrderDetailId;
            public decimal OrderDetailTotal;
        }

        [Test]
        public void I_Can_Map_Nested_Types_With_Multiple_Identifiers_And_Resolve_Duplicate_Entries_Properly()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>
                                 {
                                     { "CustomerId", 1 },
                                     { "FirstName", "Bob" },
                                     { "LastName", "Smith" },
                                     { "Orders_OrderId", 1 },
                                     { "Orders_OrderTotal", 50.50m },
                                     { "Orders_OrderDetails_OrderDetailId", 1 },
                                     { "Orders_OrderDetails_OrderDetailTotal", 25.00m }
                                 };

            var dictionary2 = new Dictionary<string, object>
                                 {
                                     { "CustomerId", 1 },
                                     { "FirstName", "Bob" },
                                     { "LastName", "Smith" },
                                     { "Orders_OrderId", 1 },
                                     { "Orders_OrderTotal", 50.50m },
                                     { "Orders_OrderDetails_OrderDetailId", 2 },
                                     { "Orders_OrderDetails_OrderDetailTotal", 25.50m }
                                 };

            var dictionary3 = new Dictionary<string, object>
                                 {
                                     { "CustomerId", 1 + 1 },
                                     { "FirstName", "Bob" },
                                     { "LastName", "Smith" },
                                     { "Orders_OrderId", 1 },
                                     { "Orders_OrderTotal", 50.50m },
                                     { "Orders_OrderDetails_OrderDetailId", 2 },
                                     { "Orders_OrderDetails_OrderDetailTotal", 25.50m }
                                 };

            var list = new List<IDictionary<string, object>> { dictionary, dictionary2, dictionary3 };

            // Act
            var customers = Slapper.AutoMapper.Map<Customer>(list).ToArray();

            // Assert

            Assert.That(customers.Count() == 2);
            Assert.That(customers.FirstOrDefault().Orders.Length == 1); // There should only be a single Order
            Assert.That(customers.FirstOrDefault().Orders.FirstOrDefault().OrderDetails.Length == 2); // There should be two OrderDetails
        }

        //[Test]
        //public void Can_Detect_Duplicate_Parent_Members_And_Properly_Instantiate_The_Object_Only_Once()
        //{
        //    // Arrange
        //    const int id = 1;
        //    const string name = "IT";
        //    const int firstEmployeeId = 1;
        //    const string firstEmployeeLastName = "Smith";

        //    var firstDictionary = new Dictionary<string, object>
        //    {
        //        {"Id", id},
        //        {"Name", name},
        //        {"Staff_Id", firstEmployeeId},
        //        {"Staff_LastName", firstEmployeeLastName}
        //    };

        //    var secondDictionary = new Dictionary<string, object>
        //    {
        //        {"Id", id},
        //        {"Name", name},
        //        {"Staff_Id", firstEmployeeId},
        //        {"Staff_LastName", firstEmployeeLastName}
        //    };

        //    // Act
        //    var department =
        //        AutoMapper.Map<Department>(new List<IDictionary<string, object>> { firstDictionary, secondDictionary }).Single();

        //    // Assert
        //    Assert.That(department.Id == id);
        //    Assert.That(department.Name == name);
        //    Assert.That(department.Staff.Count() == 2);
        //    Assert.That(department.Staff[0].Id == firstEmployeeId);
        //    Assert.That(department.Staff[0].LastName == firstEmployeeLastName);
        //}
    }
}