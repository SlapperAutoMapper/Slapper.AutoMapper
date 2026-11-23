using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Slapper
{
    public static partial class AutoMapper
    {
        /// <summary>
        /// Contains the methods and members responsible for this libraries caching concerns.
        /// </summary>
        public static class Cache
        {
            /// <summary>
            /// Cache of TypeMaps containing the types identifiers and PropertyInfo/FieldInfo objects.
            /// </summary>
            internal static readonly ConcurrentDictionary<Type, TypeMap> TypeMapCache = new ConcurrentDictionary<Type, TypeMap>();

            /// <summary>
            /// A TypeMap holds data relevant for a particular Type.
            /// </summary>
            internal class TypeMap
            {
                /// <summary>
                /// Creates a new <see cref="TypeMap"/>.
                /// </summary>
                /// <param name="type">Type to map.</param>
                /// <param name="identifiers">The <paramref name="type"/>s identifiers.</param>
                /// <param name="propertiesAndFields">The <paramref name="type"/>s properties and fields.</param>
                public TypeMap(Type type, IEnumerable<string> identifiers, Dictionary<string, object> propertiesAndFields)
                {
                    Type = type;
                    Identifiers = identifiers;
                    PropertiesAndFieldsInfo = propertiesAndFields;
                }

                /// <summary>
                /// Type for this TypeMap
                /// </summary>
                public readonly Type Type;

                /// <summary>
                /// List of identifiers
                /// </summary>
                public IEnumerable<string> Identifiers;

                /// <summary>
                /// Property/field names and their corresponding PropertyInfo/FieldInfo objects
                /// </summary>
                public Dictionary<string, object> PropertiesAndFieldsInfo;
            }

            /// <summary>
            /// <summary>
            /// Clears all internal caches.
            /// </summary>
            public static void ClearAllCaches()
            {
                TypeMapCache.Clear();
            }

            /// <summary>
            /// Obsolete. Instance caches are now scoped to each mapping call (or to an explicit <see cref="InstanceCache"/>), so there is nothing to clear.
            /// </summary>
            [Obsolete("Instance caches are scoped per mapping call. Use InstanceCache.Clear() on an explicit InstanceCache instead.")]
            public static void ClearInstanceCache()
            {
            }
        }
    }
}
