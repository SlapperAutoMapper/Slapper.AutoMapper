using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Reflection;

namespace Slapper.Tests
{
    public class TypeActivatorTests : TestBase
    {
        public interface IClass
        {
            string Id { get; set; }

            INestedClass Nested { get; set; }
        }

        public class Class : IClass
        {
            public string Id { get; set; }

            public INestedClass Nested { get; set; }
        }

        public interface INestedClass
        {
            int Count { get; set; }

            Enum Enum { get; set; }
        }

        public class NestedClass : INestedClass
        {
            public int Count { get; set; }

            public Enum Enum { get; set; }
        }

        public enum Enum
        {
            One,
            Two
        }

        public class TypeActivator : AutoMapper.Configuration.ITypeActivator
        {
            private readonly IDictionary<Type, Type> supportedTypes = new Dictionary<Type, Type>
                {
                    { typeof(IClass), typeof(Class) },
                    { typeof(INestedClass), typeof(NestedClass) }
                };

            public object Create(Type type)
            {
                var concreteType = this.supportedTypes[type];
                return Activator.CreateInstance(concreteType);
            }

            public bool CanCreate(Type type)
            {
                return this.supportedTypes.ContainsKey(type);
            }

            public int Order { get; } = 1;
        }

        [Test]
        public void Can_Use_Registered_TypeActivators_WithInterfaces()
        {
            // Arrange
            const string id = "1";
            const int count = 2;
            const Enum enu = Enum.Two;

            var dictionary = new Dictionary<string, object>
                {
                    { "Id", id },
                    { "Nested_Count", count },
                    { "Nested_Enum", enu }
                };

            AutoMapper.Configuration.TypeActivators.Add(new TypeActivator());

            // Act
            var result = Slapper.AutoMapper.Map<IClass>(dictionary);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Id == id);
            Assert.That(result.Nested.Enum == enu);
            Assert.That(result.Nested.Count == count);
        }

        public interface IClass2
        {
            string Id { get; set; }
        }

        public class Class2 : IClass2
        {
            public string Id { get; set; }
        }

        public class Class2Copy : IClass2
        {
            public string Id { get; set; }
        }

        public class Class2TypeActivator : AutoMapper.Configuration.ITypeActivator
        {
            public object Create(Type type)
            {
                return new Class2();
            }

            public bool CanCreate(Type type)
            {
                return type.IsAssignableFrom(typeof(IClass2));
            }

            public int Order { get; } = 2;
        }

        public class Class2CopyTypeActivator : AutoMapper.Configuration.ITypeActivator
        {
            public object Create(Type type)
            {
                return new Class2Copy();
            }

            public bool CanCreate(Type type)
            {
                return type.IsAssignableFrom(typeof(IClass2));
            }

            public int Order { get; } = 1;
        }

        [Test]
        public void TypeActivator_Order_Is_Adhered_To()
        {
            // Arrange
            const string id = "1";
            const int count = 2;
            const Enum enu = Enum.Two;

            var dictionary = new Dictionary<string, object>
                {
                    { "Id", id },
                    { "Nested_Count", count },
                    { "Nested_Enum", enu }
                };

            AutoMapper.Configuration.TypeActivators.Add(new Class2TypeActivator());
            AutoMapper.Configuration.TypeActivators.Add(new Class2CopyTypeActivator());

            // Act
            var result = Slapper.AutoMapper.Map<IClass2>(dictionary);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Id == id);
            Assert.That(result.GetType() == typeof(Class2Copy));
        }
    }
}
