using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Slapper.Tests
{
    [TestFixture]
    public class InstanceCacheIsolationTests : TestBase
    {
        public class Department
        {
            public int Id { get; set; }
        }

        public class Employee
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Department Department { get; set; }
        }

        public class OrderItem
        {
            public int Id { get; set; }
        }

        public class Order
        {
            public int Id { get; set; }
            public List<OrderItem> OrderItems { get; set; }
        }

        private static List<object> DynamicOrderRows(int parents, int childrenPerParent)
        {
            var rows = new List<object>();
            for (var p = 1; p <= parents; p++)
                for (var c = 1; c <= childrenPerParent; c++)
                {
                    IDictionary<string, object> row = new ExpandoObject();
                    row["Id"] = p;
                    row["OrderItems_Id"] = p * 1000 + c;
                    rows.Add(row);
                }
            return rows;
        }

        private static IDictionary<string, object> Row(params KeyValuePair<string, object>[] values)
        {
            IDictionary<string, object> row = new ExpandoObject();
            foreach (var pair in values) row[pair.Key] = pair.Value;
            return row;
        }

        private static KeyValuePair<string, object> P(string key, object value)
        {
            return new KeyValuePair<string, object>(key, value);
        }

        private static bool AreOrdersCorrect(Order[] orders)
        {
            return orders != null
                && orders.Length == 50
                && orders.All(o => o.OrderItems?.Count == 3 && o.OrderItems.Select(i => i.Id).Distinct().Count() == 3);
        }

        private static int RunConcurrentMappings(bool keepCache, bool parentFlowHasCache)
        {
            var bad = 0;
            for (var i = 0; i < 50; i++)
            {
                if (parentFlowHasCache)
                {
                    Slapper.AutoMapper.MapDynamic<Order>(DynamicOrderRows(1, 1), true).ToList();
                }

                var results = new Order[16][];
                Parallel.For(0, 16, k =>
                {
                    try { results[k] = Slapper.AutoMapper.MapDynamic<Order>(DynamicOrderRows(50, 3), keepCache).ToArray(); }
                    catch { results[k] = null; }
                });
                bad += results.Count(r => !AreOrdersCorrect(r));
            }
            return bad;
        }

        [Test]
        public void MapDynamic_Concurrent_KeepCache_False_Produces_Correct_Results()
        {
            Assert.AreEqual(0, RunConcurrentMappings(keepCache: false, parentFlowHasCache: false));
        }

        [Test]
        public void MapDynamic_Concurrent_KeepCache_False_With_Parent_Flow_Mapping_Produces_Correct_Results()
        {
            Assert.AreEqual(0, RunConcurrentMappings(keepCache: false, parentFlowHasCache: true));
        }

        [Test]
        public void MapDynamic_Concurrent_KeepCache_True_With_Parent_Flow_Mapping_Produces_Correct_Results()
        {
            Assert.AreEqual(0, RunConcurrentMappings(keepCache: true, parentFlowHasCache: true));
        }

        [Test]
        public void MapDynamic_Concurrent_Tasks_Produce_Correct_Results()
        {
            var tasks = Enumerable.Range(0, 16)
                .Select(_ => Task.Run(() => Slapper.AutoMapper.MapDynamic<Order>(DynamicOrderRows(50, 3), false).ToArray()))
                .ToArray();
            Task.WaitAll(tasks);

            Assert.IsTrue(tasks.All(t => AreOrdersCorrect(t.Result)));
        }

        [Test]
        public void MapDynamic_Separate_Calls_Return_Different_Instances_With_KeepCache_True()
        {
            var row1 = Row(P("Id", 1), P("Name", "John"));
            var row2 = Row(P("Id", 1), P("Name", "555-1234"));

            var user = Slapper.AutoMapper.MapDynamic<Employee>(new List<object> { row1 }, true).Single();
            var phone = Slapper.AutoMapper.MapDynamic<Employee>(new List<object> { row2 }, true).Single();

            Assert.AreNotSame(user, phone);
            Assert.AreEqual("John", user.Name);
            Assert.AreEqual("555-1234", phone.Name);
        }

        [Test]
        public void MapDynamic_With_Shared_InstanceCache_Merges_Across_Calls()
        {
            var row1 = Row(P("Id", 1), P("Name", "John"));
            var row2 = Row(P("Id", 1), P("Department_Id", 7));

            var cache = new Slapper.AutoMapper.InstanceCache();
            var first = Slapper.AutoMapper.MapDynamic<Employee>(new List<object> { row1 }, cache).Single();
            var second = Slapper.AutoMapper.MapDynamic<Employee>(new List<object> { row2 }, cache).Single();

            Assert.AreSame(first, second);
            Assert.AreEqual("John", second.Name);
            Assert.AreEqual(7, second.Department.Id);
        }

        [Test]
        public void MapDynamic_With_InstanceCache_Handles_Null_And_Empty_Input()
        {
            var cache = new Slapper.AutoMapper.InstanceCache();

            Assert.IsEmpty(Slapper.AutoMapper.MapDynamic<Employee>((IEnumerable<object>)null, cache));
            Assert.IsEmpty(Slapper.AutoMapper.MapDynamic<Employee>(new List<object>(), cache));
        }

        [Test]
        public void Map_With_Null_InstanceCache_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Slapper.AutoMapper.Map<Employee>(new List<IDictionary<string, object>>(), (Slapper.AutoMapper.InstanceCache)null));
        }

        [Test]
        public void One_To_Many_Rows_Are_Merged_Within_A_Single_Call()
        {
            var orders = Slapper.AutoMapper.MapDynamic<Order>(DynamicOrderRows(5, 3), false).ToList();

            Assert.AreEqual(5, orders.Count);
            Assert.IsTrue(orders.All(o => o.OrderItems.Count == 3));
        }
    }
}
