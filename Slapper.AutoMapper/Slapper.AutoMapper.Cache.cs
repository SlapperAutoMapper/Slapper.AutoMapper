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
            /// The name of the instance cache stored in the logical call context.
            /// </summary>
            internal const string InstanceCacheContextStorageKey = "Slapper.AutoMapper.InstanceCache";

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
            /// Clears all internal caches.
            /// </summary>
            public static void ClearAllCaches()
            {
                TypeMapCache.Clear();
                ClearInstanceCache();
            }

            /// <summary>
            /// Clears the instance cache. This cache contains all objects created by Slapper.AutoMapper.
            /// </summary>
            public static void ClearInstanceCache()
            {
                InternalHelpers.ContextStorage.Remove(InstanceCacheContextStorageKey);
            }

            /// <summary>
            /// Gets the instance cache containing all objects created by Slapper.AutoMapper.
            /// This cache exists for the lifetime of the current thread until manually cleared/purged.
            /// </summary>
            /// <remarks>
            /// Due to the nature of how the cache is persisted, each new thread will receive it's own
            /// unique cache.
            /// </remarks>
            /// <returns>Instance Cache</returns>
            internal static Dictionary<InternalHelpers.InstanceKey,object> GetInstanceCache()
            {
                var instanceCache = InternalHelpers.ContextStorage.Get<Dictionary<InternalHelpers.InstanceKey, object>>(InstanceCacheContextStorageKey);

                if (instanceCache == null)
                {
                    instanceCache = new Dictionary<InternalHelpers.InstanceKey, object>();

                    InternalHelpers.ContextStorage.Store(InstanceCacheContextStorageKey, instanceCache);
                }

                return instanceCache;
            }
        }
    }
}
