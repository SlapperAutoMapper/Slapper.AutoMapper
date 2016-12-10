using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Slapper.Tests
{
    [TestFixture]
    public class MapIgnoreOthersIds: TestBase
    {
        public class CustomObject
        {
            public int Test1Id;
            public int Test2Id;
            public int Test3Id;
        }

        [Test]
        public void Can_Ignore_Others_Ids()
        {
            var dictionaryList = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "Test1Id", 1 },
                    { "Test2Id", 2 },
                    { "Test3Id", 3 }
                },
                new Dictionary<string, object>
                {
                    { "Test1Id", 2 },
                    { "Test2Id", 2 },
                    { "Test3Id", 4 }
                },
                new Dictionary<string, object>
                {
                    { "Test1Id", 2 },
                    { "Test2Id", 2 },
                    { "Test3Id", 5 }
                }
            };

            var customList = AutoMapper.Map<CustomObject>(dictionaryList);
            Assert.That(customList.Any());
            Assert.That(customList.Count() == 3);
        }
    }
}
