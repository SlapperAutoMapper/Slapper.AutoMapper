using System;
using NUnit.Framework;
using static Slapper.AutoMapper.Configuration;

namespace Slapper.Tests.ConvertersTests
{
    [TestFixture]
    public class GuidConverterTests
    {
        private readonly GuidConverter converter = new GuidConverter();

        [TestCase(typeof(Guid))]
        [TestCase(typeof(Guid?))]
        public void Can_Convert_To_Type(Type targetType)
        {
            // Act + Assert
            Assert.True(this.converter.CanConvert(null, targetType)); // Input value does not matter, null is enough for the test.
        }
    }
}