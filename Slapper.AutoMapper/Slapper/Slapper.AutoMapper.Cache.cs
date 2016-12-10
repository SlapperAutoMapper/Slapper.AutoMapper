/*  Slapper.AutoMapper v1.0.0.6 ( https://github.com/SlapperAutoMapper/Slapper.AutoMapper )

    MIT License:
   
    Copyright (c) 2016, Randy Burden ( http://randyburden.com ) and contributors. All rights reserved.
    All rights reserved.

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
    associated documentation files (the "Software"), to deal in the Software without restriction, including 
    without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
    copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
    following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial 
    portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
    LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN 
    NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
    SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 

    Description:
    
    Slapper.AutoMapper maps dynamic data to static types. Slap your data into submission!
    
    Slapper.AutoMapper ( Pronounced Slapper-Dot-Automapper ) is a single file mapping library that can convert 
    dynamic data into static types and populate complex nested child objects.
    It primarily converts C# dynamics and IDictionary<string, object> to strongly typed objects and supports
    populating an entire object graph by using underscore notation to underscore into nested objects.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Slapper
{
    public static partial class AutoMapper
    {
        #region Cache

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
            /// Due to the nature of how the cache is persisted, each new thread will recieve it's own
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

        #endregion Cache
    }
}
