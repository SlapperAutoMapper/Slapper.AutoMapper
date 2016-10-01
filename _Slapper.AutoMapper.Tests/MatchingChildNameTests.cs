using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Slapper.Tests
{
    public class MatchingChildNameTests
    {
        public class SubMember
        {
            public int Id { get; set; }
        }

        public class Member
        {
            public int Id { get; set; }
            public IList<SubMember> SubMembers { get; set; }
        }

        public class Club
        {
            public int Id { get; set; }
            public IList<Member> Members { get; set; }
        }

        [Test]
        public void Can_map_grandchild_with_parts_of_same_name_as_child()
        {
            var data = new List<Dictionary<string, object>> {
                new Dictionary<string, object> {
                    {"Id", 1},
                    {"Members_Id", 1},
                    {"Members_SubMembers_Id", 1}
                }
            };

            var club = AutoMapper.MapDynamic<Club>(data).Single();

            Assert.That(club.Members.Count == 1);
            Assert.NotNull(club.Members[0].SubMembers);
            Assert.That(club.Members[0].SubMembers.Count == 1);
        }
    }
}