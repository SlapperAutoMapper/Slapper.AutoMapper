using System;
using System.Collections.Generic;
using System.Linq;

namespace Slapper
{
    /// <summary>
    /// Provides auto-mapping to static type capabilities for ORMs. Slap your ORM into submission.
    /// </summary>
    public static partial class AutoMapper
    {
        /// <summary>
        /// Attribute for specifying that a field or property is an identifier.
        /// </summary>
        [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property)]
        public class Id : Attribute
        {
        }

        /// <summary>
        /// Converts a dynamic object to a type <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and auto-map to</typeparam>
        /// <param name="dynamicObject">Dynamic list of property names and values</param>
        /// <param name="keepCache">If false, clears instance cache after mapping is completed. Defaults to true, meaning instances are kept between calls.</param>
        /// <returns>The type <typeparamref name="T"/></returns>
        /// <exception cref="ArgumentException">Exception that is thrown when the <paramref name="dynamicObject"/> cannot be converted to an IDictionary of type string and object.</exception>
        public static T MapDynamic<T>( object dynamicObject, bool keepCache = true)
        {
            return (T)MapDynamic(typeof(T), dynamicObject, keepCache);
        }

        /// <summary>
        /// Converts a dynamic object to a specified Type.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <param name="type">Type to instantiate and auto-map to</param>
        /// <param name="dynamicObject">Dynamic list of property names and values</param>
        /// <param name="keepCache">If false, clears instance cache after mapping is completed. Defaults to true, meaning instances are kept between calls.</param>
        /// <returns>The specified Type</returns>
        /// <exception cref="ArgumentException">Exception that is thrown when the <paramref name="dynamicObject"/> cannot be converted to an IDictionary of type string and object.</exception>
        public static object MapDynamic(Type type, object dynamicObject, bool keepCache = true)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (dynamicObject == null)
            {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }

            var dictionary = dynamicObject as IDictionary<string, object>;

            if (dictionary == null)
                throw new ArgumentException("Object type cannot be converted to an IDictionary<string,object>", nameof(dynamicObject));

            var propertiesList = new List<IDictionary<string, object>> { dictionary };

            return Map(type, propertiesList, keepCache).FirstOrDefault();
        }

        /// <summary>
        /// Converts a list of dynamic objects to a list of type of <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and auto-map to</typeparam>
        /// <param name="dynamicListOfProperties">Dynamic list of property names and values</param>
        /// <param name="keepCache">If false, clears instance cache after mapping is completed. Defaults to true, meaning instances are kept between calls.</param>
        /// <returns>List of type <typeparamref name="T"/></returns>
        /// <exception cref="ArgumentException">Exception that is thrown when the <paramref name="dynamicListOfProperties"/> cannot be converted to an IDictionary of type string and object.</exception>
        public static IEnumerable<T> MapDynamic<T>( IEnumerable<object> dynamicListOfProperties, bool keepCache = true)
        {
            return MapDynamic(typeof(T), dynamicListOfProperties, keepCache).Cast<T>();
        }

        /// <summary>
        /// Converts a list of dynamic objects to a list of specified Type.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <param name="type">Type to instantiate and auto-map to</param>
        /// <param name="dynamicListOfProperties">Dynamic list of property names and values</param>
        /// <param name="keepCache">If false, clears instance cache after mapping is completed. Defaults to true, meaning instances are kept between calls.</param>
        /// <returns>List of specified Type</returns>
        /// <exception cref="ArgumentException">Exception that is thrown when the <paramref name="dynamicListOfProperties"/> cannot be converted to an IDictionary of type string and object.</exception>
        public static IEnumerable<object> MapDynamic(Type type, IEnumerable<object> dynamicListOfProperties, bool keepCache = true)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (dynamicListOfProperties == null)
                return new List<object>();

            var dictionary = dynamicListOfProperties.Select(dynamicItem => dynamicItem as IDictionary<string, object>).ToList();

            if (dictionary == null)
                throw new ArgumentException("Object types cannot be converted to an IDictionary<string,object>", nameof(dynamicListOfProperties));

            if (dictionary.Count == 0 || dictionary[0] == null)
                return new List<object>();

            return Map(type, dictionary, keepCache);
        }

        /// <summary>
        /// Converts a dictionary of property names and values to a type <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and auto-map to</typeparam>
        /// <param name="listOfProperties">List of property names and values</param>
        /// <param name="keepCache">If false, clears instance cache after mapping is completed. Defaults to true, meaning instances are kept between calls.</param>
        /// <returns>The type <typeparamref name="T"/></returns>
        public static T Map<T>( IDictionary<string, object> listOfProperties, bool keepCache = true)
        {
            return (T)Map(typeof(T), listOfProperties, keepCache);
        }

        /// <summary>
        /// Converts a dictionary of property names and values to a specified Type.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <param name="type">Type to instantiate and auto-map to</param>
        /// <param name="listOfProperties">List of property names and values</param>
        /// <param name="keepCache">If false, clears instance cache after mapping is completed. Defaults to true, meaning instances are kept between calls.</param>
        /// <returns>The specified Type</returns>
        public static object Map(Type type, IDictionary<string, object> listOfProperties, bool keepCache = true)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var propertiesList = new List<IDictionary<string, object>> { listOfProperties };

            return Map(type, propertiesList, keepCache).FirstOrDefault();
        }

        /// <summary>
        /// Converts a list of dictionaries of property names and values to a list of type of <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and auto-map to</typeparam>
        /// <param name="listOfProperties">List of property names and values</param>
        /// <param name="keepCache">If false, clears instance cache after mapping is completed. Defaults to true, meaning instances are kept between calls.</param>
        /// <returns>List of type <typeparamref name="T"/></returns>
        public static IEnumerable<T> Map<T>( IEnumerable<IDictionary<string, object>> listOfProperties, bool keepCache = true)
        {
            return Map(typeof(T), listOfProperties, keepCache).Cast<T>();
        }

        /// <summary>
        /// Converts a list of dictionaries of property names and values to a list of specified Type.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <param name="type">Type to instantiate and auto-map to</param>
        /// <param name="listOfProperties">List of property names and values</param>
        /// <param name="keepCache">If false, clears instance cache after mapping is completed. Defaults to true, meaning instances are kept between calls.</param>
        /// <returns>List of specified Type</returns>
        public static IEnumerable<object> Map(Type type, IEnumerable<IDictionary<string, object>> listOfProperties, bool keepCache = true)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var instanceCache = new Dictionary<object, object>();

            foreach (var properties in listOfProperties)
            {
                var getInstanceResult = InternalHelpers.GetInstance(type, properties);

                object instance = getInstanceResult.Item2;

                var key = getInstanceResult.Item3;

                if (instanceCache.ContainsKey(key) == false)
                {
                    instanceCache.Add(key, instance);
                }

                var caseInsensitiveDictionary = new Dictionary<string, object>(properties, StringComparer.OrdinalIgnoreCase);

                InternalHelpers.Map(caseInsensitiveDictionary, instance);
            }

            if (!keepCache)
                Cache.ClearInstanceCache();

            return instanceCache.Select(pair => pair.Value);
        }
    }
}
