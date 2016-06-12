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
using System.Collections.Generic;
using System.Linq;

namespace Slapper
{
    /// <summary>
    /// Provides auto-mapping to static type capabilities for ORMs. Slap your ORM into submission.
    /// </summary>
    public static partial class AutoMapper
    {
        #region Attributes

        /// <summary>
        /// Attribute for specifying that a field or property is an identifier.
        /// </summary>
        [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false )]
        public class Id : Attribute
        {
        }

        #endregion Attributes

        #region Mapping

        /// <summary>
        /// Converts a dynamic object to a type <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and automap to</typeparam>
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
        /// <param name="type">Type to instantiate and automap to</param>
        /// <param name="dynamicObject">Dynamic list of property names and values</param>
        /// <param name="keepCache">If false, clears instance cache after mapping is completed. Defaults to true, meaning instances are kept between calls.</param>
        /// <returns>The specified Type</returns>
        /// <exception cref="ArgumentException">Exception that is thrown when the <paramref name="dynamicObject"/> cannot be converted to an IDictionary of type string and object.</exception>
        public static object MapDynamic(Type type, object dynamicObject, bool keepCache = true)
        {
            if (dynamicObject == null)
            {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }

            var dictionary = dynamicObject as IDictionary<string, object>;

            if (dictionary == null)
                throw new ArgumentException("Object type cannot be converted to an IDictionary<string,object>", "dynamicObject");

            var propertiesList = new List<IDictionary<string, object>> { dictionary };

            return Map(type, propertiesList, keepCache).FirstOrDefault();
        }

        /// <summary>
        /// Converts a list of dynamic objects to a list of type of <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and automap to</typeparam>
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
        /// <param name="type">Type to instantiate and automap to</param>
        /// <param name="dynamicListOfProperties">Dynamic list of property names and values</param>
        /// <param name="keepCache">If false, clears instance cache after mapping is completed. Defaults to true, meaning instances are kept between calls.</param>
        /// <returns>List of specified Type</returns>
        /// <exception cref="ArgumentException">Exception that is thrown when the <paramref name="dynamicListOfProperties"/> cannot be converted to an IDictionary of type string and object.</exception>
        public static IEnumerable<object> MapDynamic(Type type, IEnumerable<object> dynamicListOfProperties, bool keepCache = true)
        {
            if (dynamicListOfProperties == null)
                return new List<object>();

            var dictionary = dynamicListOfProperties.Select(dynamicItem => dynamicItem as IDictionary<string, object>).ToList();

            if (dictionary == null)
                throw new ArgumentException("Object types cannot be converted to an IDictionary<string,object>", "dynamicListOfProperties");

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
        /// <typeparam name="T">Type to instantiate and automap to</typeparam>
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
        /// <param name="type">Type to instantiate and automap to</param>
        /// <param name="listOfProperties">List of property names and values</param>
        /// <param name="keepCache">If false, clears instance cache after mapping is completed. Defaults to true, meaning instances are kept between calls.</param>
        /// <returns>The specified Type</returns>
        public static object Map(Type type, IDictionary<string, object> listOfProperties, bool keepCache = true)
        {
            var propertiesList = new List<IDictionary<string, object>> { listOfProperties };

            return Map(type, propertiesList, keepCache).FirstOrDefault();
        }

        /// <summary>
        /// Converts a list of dictionaries of property names and values to a list of type of <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and automap to</typeparam>
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
        /// <param name="type">Type to instantiate and automap to</param>
        /// <param name="listOfProperties">List of property names and values</param>
        /// <param name="keepCache">If false, clears instance cache after mapping is completed. Defaults to true, meaning instances are kept between calls.</param>
        /// <returns>List of specified Type</returns>
        public static IEnumerable<object> Map(Type type, IEnumerable<IDictionary<string, object>> listOfProperties, bool keepCache = true)
        {
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

        #endregion Mapping
    }
}
