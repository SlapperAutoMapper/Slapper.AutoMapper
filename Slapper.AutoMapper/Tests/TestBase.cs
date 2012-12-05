using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Slapper.Tests
{
    public class TestBase
    {
        [TearDown]
        public void TearDown()
        {
            Slapper.AutoMapper.Cache.ClearAllCaches();
        }
    }
}