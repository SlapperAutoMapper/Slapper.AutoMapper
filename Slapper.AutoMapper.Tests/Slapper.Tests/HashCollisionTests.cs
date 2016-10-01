using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NUnit.Framework;

namespace Slapper.Tests
{
    public class HashCollisionTests
    {
        [Test]
        public void Avoids_Hash_Collisions()
        {
            // Arrange
            var id2 = typeof (Employee).GetHashCode() - typeof (Contract).GetHashCode();

            var source = new List<object>();

            dynamic obj1 = new ExpandoObject();

            obj1.Id = 1;
            obj1.Contracts_Id = id2;

            source.Add(obj1);

            dynamic obj2 = new ExpandoObject();

            obj2.Id = 1;
            obj2.Contracts_Id = id2 + 1;

            source.Add(obj2);

            // Act/Assert
            var result = AutoMapper.MapDynamic<Employee>(source).First();
        }

        public class Employee
        {
            public int Id { get; set; }

            public List<Contract> Contracts { get; set; }

            public override int GetHashCode()
            {
                return Id;
            }
        }

        public class Contract
        {
            public int Id { get; set; }

            public override int GetHashCode()
            {
                return Id;
            }
        }
    }
}