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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("Slapper.Tests")]
namespace Slapper
{
    public static partial class AutoMapper
    {
        #region Internal Helpers

        /// <summary>
        /// Contains the methods and members responsible for this libraries internal concerns.
        /// </summary>
        internal static class InternalHelpers
        {
            /// <summary>
            /// Combine several hashcodes into a single new one. This implementation was grabbed from http://stackoverflow.com/a/34229665 where it is introduced 
            /// as MS implementation of GetHashCode() for strings.
            /// </summary>
            /// <param name="hashCodes">Hascodes to be combined.</param>
            /// <returns>A new Hascode value combining those passed as parameters.</returns>
            private static int CombineHashCodes(params int[] hashCodes)
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                int i = 0;
                foreach (var hashCode in hashCodes)
                {
                    if (i % 2 == 0)
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ hashCode;
                    else
                        hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ hashCode;

                    ++i;
                }

                return hash1 + (hash2 * 1566083941);
            }

            /// <summary>
            /// Defines the key for caching instances. Overrides Equality as to get unicity for a given set of identifiers values
            /// for a given type.
            /// </summary>
            public struct InstanceKey : IEquatable<InstanceKey>
            {
                public bool Equals(InstanceKey other) {
                    return Equals(Type, other.Type) 
                        && Equals(ParentInstance, other.ParentInstance) 
                        && StructuralComparisons.StructuralEqualityComparer.Equals(IdentifierValues, other.IdentifierValues);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    return obj is InstanceKey && Equals((InstanceKey) obj);
                }

                public override int GetHashCode()
                {
                    unchecked
                    {
                        return CombineHashCodes(Type?.GetHashCode() ?? 0, StructuralComparisons.StructuralEqualityComparer.GetHashCode(IdentifierValues), ParentInstance?.GetHashCode() ?? 0);
                    }
                }

                public static bool operator ==(InstanceKey left, InstanceKey right) { return left.Equals(right); }

                public static bool operator !=(InstanceKey left, InstanceKey right) { return !left.Equals(right); }

                public InstanceKey(Type type, object[] identifierValues, object parentInstance)
                {
                    Type = type;
                    IdentifierValues = identifierValues;
                    ParentInstance = parentInstance;
                }

                public  Type Type { get; }
                public object[] IdentifierValues { get; }
                public object ParentInstance { get;  }
            }

            /// <summary>
            /// Gets the identifiers for the given type. Returns NULL if not found.
            /// Results are cached for subsequent use and performance.
            /// </summary>
            /// <remarks>
            /// If no identifiers have been manually added, this method will attempt
            /// to first find an <see cref="Slapper.AutoMapper.Id"/> attribute on the <paramref name="type"/>
            /// and if not found will then try to match based upon any specified identifier conventions.
            /// </remarks>
            /// <param name="type">Type</param>
            /// <returns>Identifier</returns>
            public static IEnumerable<string> GetIdentifiers(Type type)
            {
                var typeMap = Cache.TypeMapCache.GetOrAdd(type, CreateTypeMap(type));

                return typeMap.Identifiers.Any() ? typeMap.Identifiers : null;
            }

            /// <summary>
            /// Get a Dictionary of a type's property names and field names and their corresponding PropertyInfo or FieldInfo.
            /// Results are cached for subsequent use and performance.
            /// </summary>
            /// <param name="type">Type</param>
            /// <returns>Dictionary of a type's property names and their corresponding PropertyInfo</returns>
            public static Dictionary<string, object> GetFieldsAndProperties(Type type)
            {
                var typeMap = Cache.TypeMapCache.GetOrAdd(type, CreateTypeMap(type));

                return typeMap.PropertiesAndFieldsInfo;
            }

            /// <summary>
            /// Creates an instance of the specified type using that type's default constructor.
            /// </summary>
            /// <param name="type">The type of object to create.</param>
            /// <returns>
            /// A reference to the newly created object.
            /// </returns>
            public static object CreateInstance(Type type)
            {
                if (type == typeof(string))
                {
                    return string.Empty;
                }

                if (Configuration.TypeActivators.Count > 0)
                {
                    foreach (var typeActivator in Configuration.TypeActivators.OrderBy(ta => ta.Order))
                    {
                        if (typeActivator.CanCreate(type))
                        {
                            return typeActivator.Create(type);
                        }
                    }
                }

                return Activator.CreateInstance(type);
            }

            /// <summary>
            /// Creates a TypeMap for a given Type.
            /// </summary>
            /// <param name="type">Type</param>
            /// <returns>TypeMap</returns>
            public static Cache.TypeMap CreateTypeMap(Type type)
            {
                var conventionIdentifiers = Configuration.IdentifierConventions.Select(applyIdentifierConvention => applyIdentifierConvention(type)).ToList();

                var fieldsAndProperties = CreateFieldAndPropertyInfoDictionary(type);

                var identifiers = new List<string>();

                foreach (var fieldOrProperty in fieldsAndProperties)
                {
                    var memberName = fieldOrProperty.Key;

                    var member = fieldOrProperty.Value;

                    var fieldInfo = member as FieldInfo;

                    if (fieldInfo != null)
                    {
                        if (fieldInfo.GetCustomAttributes(Configuration.IdentifierAttributeType, false).Length > 0)
                        {
                            identifiers.Add(memberName);
                        }
                        else if (conventionIdentifiers.Exists(x => x.ToLower() == memberName.ToLower()))
                        {
                            identifiers.Add(memberName);
                        }
                    }
                    else
                    {
                        var propertyInfo = member as PropertyInfo;

                        if (propertyInfo != null)
                        {
                            if (propertyInfo.GetCustomAttributes(Configuration.IdentifierAttributeType, false).Length > 0)
                            {
                                identifiers.Add(memberName);
                            }
                            else if (conventionIdentifiers.Exists(x => x.ToLower() == memberName.ToLower()))
                            {
                                identifiers.Add(memberName);
                            }
                        }
                    }
                }

                var typeMap = new Cache.TypeMap(type, identifiers, fieldsAndProperties);

                return typeMap;
            }

            /// <summary>
            /// Creates a Dictionary of field or property names and their corresponding FieldInfo or PropertyInfo objects
            /// </summary>
            /// <param name="type">Type</param>
            /// <returns>Dictionary of member names and member info objects</returns>
            public static Dictionary<string, object> CreateFieldAndPropertyInfoDictionary(Type type)
            {
                var dictionary = new Dictionary<string, object>();

                var properties = type.GetProperties();

                foreach (var propertyInfo in properties)
                {
                    dictionary.Add(propertyInfo.Name, propertyInfo);
                }

                var fields = type.GetFields();

                foreach (var fieldInfo in fields)
                {
                    dictionary.Add(fieldInfo.Name, fieldInfo);
                }

                return dictionary;
            }

            /// <summary>
            /// Gets the Type of the Field or Property
            /// </summary>
            /// <param name="member">FieldInfo or PropertyInfo object</param>
            /// <returns>Type</returns>
            public static Type GetMemberType(object member)
            {
                Type type = null;

                var fieldInfo = member as FieldInfo;

                if (fieldInfo != null)
                {
                    type = fieldInfo.FieldType;
                }
                else
                {
                    var propertyInfo = member as PropertyInfo;

                    if (propertyInfo != null)
                    {
                        type = propertyInfo.PropertyType;
                    }
                }

                return type;
            }

            /// <summary>
            /// Sets the value on a Field or Property
            /// </summary>
            /// <param name="member">FieldInfo or PropertyInfo object</param>
            /// <param name="obj">Object to set the value on</param>
            /// <param name="value">Value</param>
            public static void SetMemberValue(object member, object obj, object value)
            {
                var fieldInfo = member as FieldInfo;

                if (fieldInfo != null)
                {
                    value = ConvertValuesTypeToMembersType(value, fieldInfo.Name, fieldInfo.FieldType, fieldInfo.DeclaringType);

                    try
                    {
                        fieldInfo.SetValue(obj, value);
                    }
                    catch (Exception e)
                    {
                        string errorMessage =
                            string.Format("{0}: An error occurred while mapping the value '{1}' of type {2} to the member name '{3}' of type {4} on the {5} class.",
                                           e.Message, value, value.GetType(), fieldInfo.Name, fieldInfo.FieldType, fieldInfo.DeclaringType);

                        throw new Exception(errorMessage, e);
                    }
                }
                else
                {
                    var propertyInfo = member as PropertyInfo;

                    if (propertyInfo != null)
                    {
                        value = ConvertValuesTypeToMembersType(value, propertyInfo.Name, propertyInfo.PropertyType, propertyInfo.DeclaringType);

                        try
                        {
                            propertyInfo.SetValue(obj, value, null);
                        }
                        catch (Exception e)
                        {
                            string errorMessage =
                                string.Format("{0}: An error occurred while mapping the value '{1}' of type {2} to the member name '{3}' of type {4} on the {5} class.",
                                               e.Message, value, value.GetType(), propertyInfo.Name, propertyInfo.PropertyType, propertyInfo.DeclaringType);

                            throw new Exception(errorMessage, e);
                        }
                    }
                }
            }

            /// <summary>
            /// Converts the values type to the members type if needed.
            /// </summary>
            /// <param name="value">Object value.</param>
            /// <param name="memberName">Member name.</param>
            /// <param name="memberType">Member type.</param>
            /// <param name="classType">Declarying class type.</param>
            /// <returns>Value converted to the same type as the member type.</returns>
            private static object ConvertValuesTypeToMembersType(object value, string memberName, Type memberType, Type classType)
            {
                if (value == null || value == DBNull.Value)
                    return null;

                var valueType = value.GetType();

                try
                {
                    if (valueType != memberType)
                    {
                        foreach (var typeConverter in Configuration.TypeConverters.OrderBy(x => x.Order))
                        {
                            if (typeConverter.CanConvert(value, memberType))
                            {
                                var convertedValue = typeConverter.Convert(value, memberType);

                                return convertedValue;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    string errorMessage = string.Format("{0}: An error occurred while mapping the value '{1}' of type {2} to the member name '{3}' of type {4} on the {5} class.",
                                                         e.Message, value, valueType, memberName, memberType, classType);

                    throw new Exception(errorMessage, e);
                }

                return value;
            }

            /// <summary>
            /// Gets the value of the member
            /// </summary>
            /// <param name="member">FieldInfo or PropertyInfo object</param>
            /// <param name="obj">Object to get the value from</param>
            /// <returns>Value of the member</returns>
            private static object GetMemberValue(object member, object obj)
            {
                object value = null;

                var fieldInfo = member as FieldInfo;

                if (fieldInfo != null)
                {
                    value = fieldInfo.GetValue(obj);
                }
                else
                {
                    var propertyInfo = member as PropertyInfo;

                    if (propertyInfo != null)
                    {
                        value = propertyInfo.GetValue(obj, null);
                    }
                }

                return value;
            }

            /// <summary>
            /// Computes a key for storing and identifying an instance in the cache.
            /// </summary>
            /// <param name="type">Type of instance to get</param>
            /// <param name="properties">List of properties and values</param>
            /// <param name="parentInstance">Parent instance. Can be NULL if this is the root instance.</param>
            /// <returns>
            /// InstanceKey that will be unique for given set of identifiers values for the type. If the type isn't associated with any 
            /// identifier, the return value is made unique by generating a Guid.
            /// ASSUMES GetIdentifiers(type) ALWAYS RETURN IDENTIFIERS IN THE SAME ORDER FOR A GIVEN TYPE.
            /// This is certainly the case as long as GetIdentifiers caches its result for a given type (which it does by 2016-11-25).
            /// </returns>
            private static InstanceKey GetCacheKey(Type type, IDictionary<string, object> properties, object parentInstance)
            {
                var identifierValues = GetIdentifiers(type)?.Select(id => properties[id]).DefaultIfEmpty(Guid.NewGuid()).ToArray()
                    ?? new object[] { Guid.NewGuid() };

                var key = new InstanceKey(type, identifierValues, parentInstance);
                return key;
            }


            /// <summary>
            /// Gets a new or existing instance depending on whether an instance with the same identifiers already existing
            /// in the instance cache.
            /// </summary>
            /// <param name="type">Type of instance to get</param>
            /// <param name="properties">List of properties and values</param>
            /// <param name="parentInstance">Parent instance. Can be NULL if this is the root instance.</param>
            /// <returns>
            /// Tuple of bool, object, int where bool represents whether this is a newly created instance,
            /// object being an instance of the requested type and int being the instance's identifier hash.
            /// </returns>
            internal static Tuple<bool, object, InstanceKey> GetInstance(Type type, IDictionary<string, object> properties, object parentInstance = null)
            {
                var key = GetCacheKey(type, properties, parentInstance);

                var instanceCache = Cache.GetInstanceCache();

                object instance;

                var isNewlyCreatedInstance = !instanceCache.TryGetValue(key, out instance);

                if (isNewlyCreatedInstance)
                {
                    instance = CreateInstance(type);
                    instanceCache[key] = instance;
                }

                return Tuple.Create(isNewlyCreatedInstance, instance, key);
            }

            /// <summary>
            /// Populates the given instance's properties where the IDictionary key property names
            /// match the type's property names case insensitively.
            /// 
            /// Population of complex nested child properties is supported by underscoring "_" into the
            /// nested child properties in the property name.
            /// </summary>
            /// <param name="dictionary">Dictionary of property names and values</param>
            /// <param name="instance">Instance to populate</param>
            /// <param name="parentInstance">Optional parent instance of the instance being populated</param>
            /// <returns>Populated instance</returns>
            internal static object Map(IDictionary<string, object> dictionary, object instance, object parentInstance = null)
            {
                if (instance.GetType().IsPrimitive || instance is string)
                {
                    object value;
                    if (!dictionary.TryGetValue("$", out value))
                    {
                        throw new InvalidCastException("For lists of primitive types, include $ as the name of the property");
                    }

                    instance = value;
                    return instance;
                }

                var fieldsAndProperties = GetFieldsAndProperties(instance.GetType());

                foreach (var fieldOrProperty in fieldsAndProperties)
                {
                    var memberName = fieldOrProperty.Key.ToLower();

                    var member = fieldOrProperty.Value;

                    object value;

                    // Handle populating simple members on the current type
                    if (dictionary.TryGetValue(memberName, out value))
                    {
                        SetMemberValue(member, instance, value);
                    }
                    else
                    {
                        Type memberType = GetMemberType(member);

                        // Handle populating complex members on the current type
                        if (memberType.IsClass || memberType.IsInterface)
                        {
                            // Try to find any keys that start with the current member name
                            var nestedDictionary = dictionary.Where(x => x.Key.ToLower().StartsWith(memberName + "_")).ToList();

                            // If there weren't any keys
                            if (!nestedDictionary.Any())
                            {
                                // And the parent instance was not null
                                if (parentInstance != null)
                                {
                                    // And the parent instance is of the same type as the current member
                                    if (parentInstance.GetType() == memberType)
                                    {
                                        // Then this must be a 'parent' to the current type
                                        SetMemberValue(member, instance, parentInstance);
                                    }
                                }

                                continue;
                            }
                            var regex = new Regex(Regex.Escape(memberName + "_"));
                            var newDictionary = nestedDictionary.ToDictionary(pair => regex.Replace(pair.Key.ToLower(), string.Empty, 1),
                                pair => pair.Value, StringComparer.OrdinalIgnoreCase);

                            // Try to get the value of the complex member. If the member
                            // hasn't been initialized, then this will return null.
                            object nestedInstance = GetMemberValue(member, instance);

                            var genericCollectionType = typeof(IEnumerable<>);
                            var isEnumerableType = memberType.IsGenericType && genericCollectionType.IsAssignableFrom(memberType.GetGenericTypeDefinition())
                                                   || memberType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericCollectionType);

                            // If the member is null and is a class or interface (not ienumerable), try to create an instance of the type
                            if (nestedInstance == null && (memberType.IsClass || (memberType.IsInterface && !isEnumerableType)))
                            {
                                if (memberType.IsArray)
                                {
                                    nestedInstance = new ArrayList().ToArray(memberType.GetElementType());
                                }
                                else
                                {
                                    nestedInstance = typeof(IEnumerable).IsAssignableFrom(memberType)
                                                         ? CreateInstance(memberType)
                                                         : GetInstance(memberType, newDictionary, parentInstance == null ? 0 : parentInstance.GetHashCode()).Item2;
                                }
                            }

                            if (isEnumerableType)
                            {
                                var innerType = memberType.GetGenericArguments().FirstOrDefault() ?? memberType.GetElementType();
                                nestedInstance = MapCollection(innerType, newDictionary, nestedInstance, instance);
                            }
                            else
                            {
                                if (newDictionary.Values.All(v => v == null))
                                {
                                    nestedInstance = null;
                                }
                                else
                                {
                                    nestedInstance = Map(newDictionary, nestedInstance, instance);
                                }
                            }

                            SetMemberValue(member, instance, nestedInstance);
                        }
                    }
                }

                return instance;
            }

            /// <summary>
            /// Populates the given instance's properties where the IDictionary key property names
            /// match the type's property names case insensitively.
            /// 
            /// Population of complex nested child properties is supported by underscoring "_" into the
            /// nested child properties in the property name.
            /// </summary>
            /// <param name="type">Underlying instance type</param>
            /// <param name="dictionary">Dictionary of property names and values</param>
            /// <param name="instance">Instance to populate</param>
            /// <param name="parentInstance">Optional parent instance of the instance being populated</param>
            /// <returns>Populated instance</returns>
            internal static object MapCollection(Type type, IDictionary<string, object> dictionary, object instance, object parentInstance = null)
            {
                Type baseListType = typeof(List<>);
                Type collectionType = instance == null ? baseListType.MakeGenericType(type) : instance.GetType();

                if (instance == null)
                {
                    instance = CreateInstance(collectionType);
                }

                // If the dictionnary only contains null values, we return an empty instance
                if (dictionary.Values.FirstOrDefault(v => v != null) == null)
                {
                    return instance;
                }

                var getInstanceResult = GetInstance(type, dictionary, parentInstance);

                // Is this a newly created instance? If false, then this item was retrieved from the instance cache.
                bool isNewlyCreatedInstance = getInstanceResult.Item1;

                bool isArray = instance.GetType().IsArray;

                object instanceToAddToCollectionInstance = getInstanceResult.Item2;

                instanceToAddToCollectionInstance = Map(dictionary, instanceToAddToCollectionInstance, parentInstance);

                if (isNewlyCreatedInstance)
                {
                    if (isArray)
                    {
                        var arrayList = new ArrayList { instanceToAddToCollectionInstance };

                        instance = arrayList.ToArray(type);
                    }
                    else
                    {
                        MethodInfo addMethod = collectionType.GetMethod("Add");

                        addMethod.Invoke(instance, new[] { instanceToAddToCollectionInstance });
                    }
                }
                else
                {
                    MethodInfo containsMethod = collectionType.GetMethod("Contains");

                    var alreadyContainsInstance = (bool)containsMethod.Invoke(instance, new[] { instanceToAddToCollectionInstance });

                    if (alreadyContainsInstance == false)
                    {
                        if (isArray)
                        {
                            var arrayList = new ArrayList((ICollection)instance);

                            instance = arrayList.ToArray(type);
                        }
                        else
                        {
                            MethodInfo addMethod = collectionType.GetMethod("Add");

                            addMethod.Invoke(instance, new[] { instanceToAddToCollectionInstance });
                        }
                    }
                }

                return instance;
            }

            /// <summary>
            /// Provides a means of getting/storing data in the host application's
            /// appropriate context.
            /// </summary>
            internal interface IContextStorage
            {
                /// <summary>
                /// Get a stored item.
                /// </summary>
                /// <typeparam name="T">Object type</typeparam>
                /// <param name="key">Item key</param>
                /// <returns>Reference to the requested object</returns>
                T Get<T>(string key);

                /// <summary>
                /// Stores an item.
                /// </summary>
                /// <param name="key">Item key</param>
                /// <param name="obj">Object to store</param>
                void Store(string key, object obj);

                /// <summary>
                /// Removes an item.
                /// </summary>
                /// <param name="key">Item key</param>
                void Remove(string key);
            }

            /// <summary>
            /// Provides a means of getting/storing data in the host application's
            /// appropriate context.
            /// </summary>
            /// <remarks>
            /// For ASP.NET applications, it will store in the data in the current HTTPContext.
            /// For all other applications, it will store the data in the logical call context.
            /// </remarks>
            public class InternalContextStorage : IContextStorage
            {
                /// <summary>
                /// Get a stored item.
                /// </summary>
                /// <typeparam name="T">Object type</typeparam>
                /// <param name="key">Item key</param>
                /// <returns>Reference to the requested object</returns>
                public T Get<T>(string key)
                {
                    try
                    {
                        if (ReflectionHelper.HttpContext.GetCurrentHttpContext() == null)
                        {
                            return (T)CallContext.GetData(key);
                        }

                        return ReflectionHelper.HttpContext.GetItemFromHttpContext<T>(key);
                    }
                    catch (Exception ex)
                    {
                        Logging.Logger.Log(Logging.LogLevel.Error, ex, "An error occurred in ContextStorage.Get() retrieving key: {0} for type: {1}.", key, typeof(T));
                    }

                    return default(T);
                }

                /// <summary>
                /// Stores an item.
                /// </summary>
                /// <param name="key">Item key</param>
                /// <param name="obj">Object to store</param>
                public void Store(string key, object obj)
                {
                    if (ReflectionHelper.HttpContext.GetCurrentHttpContext() == null)
                    {
                        CallContext.SetData(key, obj);
                    }
                    else
                    {
                        ReflectionHelper.HttpContext.StoreItemInHttpContext(key, obj);
                    }
                }

                /// <summary>
                /// Removes an item.
                /// </summary>
                /// <param name="key">Item key</param>
                public void Remove(string key)
                {
                    if (ReflectionHelper.HttpContext.GetCurrentHttpContext() == null)
                    {
                        CallContext.FreeNamedDataSlot(key);
                    }
                    else
                    {
                        ReflectionHelper.HttpContext.RemoveItemFromHttpContext(key);
                    }
                }
            }

            /// <summary>
            /// Provides a means of getting/storing data in the host application's
            /// appropriate context.
            /// </summary>
            /// <remarks>
            /// For ASP.NET applications, it will store in the data in the current HTTPContext.
            /// For all other applications, it will store the data in the logical call context.
            /// </remarks>
            internal static class ContextStorage
            {
                /// <summary>
                /// Provides a means of getting/storing data in the host application's
                /// appropriate context.
                /// </summary>
                public static IContextStorage ContextStorageImplementation { get; set; }

                static ContextStorage()
                {
                    ContextStorageImplementation = new InternalContextStorage();
                }

                /// <summary>
                /// Get a stored item.
                /// </summary>
                /// <typeparam name="T">Object type</typeparam>
                /// <param name="key">Item key</param>
                /// <returns>Reference to the requested object</returns>
                public static T Get<T>(string key)
                {
                    return ContextStorageImplementation.Get<T>(key);
                }

                /// <summary>
                /// Stores an item.
                /// </summary>
                /// <param name="key">Item key</param>
                /// <param name="obj">Object to store</param>
                public static void Store(string key, object obj)
                {
                    ContextStorageImplementation.Store(key, obj);
                }

                /// <summary>
                /// Removes an item.
                /// </summary>
                /// <param name="key">Item key</param>
                public static void Remove(string key)
                {
                    ContextStorageImplementation.Remove(key);
                }
            }

            /// <summary>
            /// Contains the methods and members responsible for this libraries reflection concerns.
            /// </summary>
            private static class ReflectionHelper
            {
                /// <summary>
                /// Provides access to System.Web.HttpContext.Current.Items via reflection.
                /// </summary>
                public static class HttpContext
                {
                    /// <summary>
                    /// Attempts to load and cache System.Web.HttpContext.Current.Items.
                    /// </summary>
                    static HttpContext()
                    {
                        try
                        {
                            SystemDotWeb = Assembly
                                .Load("System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");

                            if (SystemDotWeb == null) return;

                            SystemDotWebDotHttpContext = SystemDotWeb.GetType("System.Web.HttpContext", false, true);

                            if (SystemDotWebDotHttpContext == null) return;

                            // Get the current HTTP context property info
                            CurrentHttpContextPropertyInfo = SystemDotWebDotHttpContext
                                .GetProperty("Current", (BindingFlags.Public | BindingFlags.Static));

                            // Get the property info for the requested property
                            ItemsPropertyInfo = SystemDotWebDotHttpContext
                                .GetProperty("Items", (BindingFlags.Public | BindingFlags.Instance));
                        }
                        catch (Exception ex)
                        {
                            Logging.Logger.Log(Logging.LogLevel.Error, ex, "An error occurred attempting to get the current HttpContext.");
                        }
                    }

                    /// <summary>
                    /// System.Web assembly reference.
                    /// </summary>
                    public static readonly Assembly SystemDotWeb;

                    /// <summary>
                    /// System.Web.HttpContext type reference.
                    /// </summary>
                    public static readonly Type SystemDotWebDotHttpContext;

                    /// <summary>
                    /// System.Web.HttpContext.Current PropertyInfo reference.
                    /// </summary>
                    public static readonly PropertyInfo CurrentHttpContextPropertyInfo;

                    /// <summary>
                    /// System.Web.HttpContext.Current.Items PropertyInfo reference.
                    /// </summary>
                    public static readonly PropertyInfo ItemsPropertyInfo;

                    /// <summary>
                    /// Retrieves an item of type <typeparamref name="T"/> from the current HttpContext.
                    /// </summary>
                    /// <remarks>
                    /// This is functionally equivalent to:
                    /// T obj = ( T ) System.Web.HttpContext.Current.Items[ "SomeKeyName" ];
                    /// </remarks>
                    /// <typeparam name="T">Type requested</typeparam>
                    /// <param name="key">Key name</param>
                    /// <returns>Requested item</returns>
                    public static T GetItemFromHttpContext<T>(string key)
                    {
                        if (SystemDotWeb != null && SystemDotWebDotHttpContext != null
                             && CurrentHttpContextPropertyInfo != null && ItemsPropertyInfo != null)
                        {
                            // Get a reference to the current HTTP context
                            object currentHttpContext = CurrentHttpContextPropertyInfo.GetValue(null, null);

                            if (currentHttpContext != null)
                            {
                                var items = ItemsPropertyInfo.GetValue(currentHttpContext, null) as IDictionary;

                                if (items != null)
                                {
                                    object value = items[key];

                                    if (value != null)
                                    {
                                        return (T)value;
                                    }
                                }
                            }
                        }

                        return default(T);
                    }

                    /// <summary>
                    /// Stores an item in the current HttpContext.
                    /// </summary>
                    /// <param name="key">Item key</param>
                    /// <param name="value">Item value</param>
                    public static void StoreItemInHttpContext(object key, object value)
                    {
                        object currentHttpContext = GetCurrentHttpContext();

                        if (currentHttpContext != null)
                        {
                            var items = ItemsPropertyInfo.GetValue(currentHttpContext, null) as IDictionary;

                            if (items != null)
                            {
                                items.Add(key, value);
                            }
                        }
                    }

                    /// <summary>
                    /// Removes an item from the current HttpContext.
                    /// </summary>
                    /// <param name="key">Item key</param>
                    public static void RemoveItemFromHttpContext(object key)
                    {
                        object currentHttpContext = GetCurrentHttpContext();

                        if (currentHttpContext != null)
                        {
                            var items = ItemsPropertyInfo.GetValue(currentHttpContext, null) as IDictionary;

                            if (items != null)
                            {
                                items.Remove(key);
                            }
                        }
                    }

                    /// <summary>
                    /// Gets the current HttpContext.
                    /// </summary>
                    /// <returns>Reference to the current HttpContext.</returns>
                    public static object GetCurrentHttpContext()
                    {
                        if (SystemDotWeb != null && SystemDotWebDotHttpContext != null
                             && CurrentHttpContextPropertyInfo != null && ItemsPropertyInfo != null)
                        {
                            // Get a reference to the current HTTP context
                            object currentHttpContext = CurrentHttpContextPropertyInfo.GetValue(null, null);

                            return currentHttpContext;
                        }

                        return null;
                    }
                }
            }
        }

        #endregion Internal Helpers
    }
}
