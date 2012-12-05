/*  SlapperAutoMapper v0.0.0.3 ( https://github.com/randyburden/SlapperAutoMapper )

    MIT License:
   
    Copyright (c) 2012, Randy Burden ( http://randyburden.com )
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
    
    SlapperAutoMapper provides auto-mapping to static type capabilities for ORMs.
    
    SlapperAutoMapper is a mapping tool that allows the conversion of an IDictionary<string, object> to a
    strongly typed object. Simply put, it allows you to convert dynamic data to strongly typed objects with 
    ease and populating complex nested child objects in your object hierarchy comes free out of the box.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace Slapper
{
    /// <summary>
    /// Provides auto-mapping to static type capabilities for ORMs. Slap your ORM into submission.
    /// </summary>
    public static class AutoMapper
    {
        static AutoMapper()
        {
            ApplyDefaultIdentifierConventions();
        }

        /// <summary>
        /// Specifies that a field or property is an identifier.
        /// </summary>
        [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false )]
        public class Id : Attribute { }

        /// <summary>
        /// Convention for finding an identifier.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public delegate string ApplyIdentifierConvention( Type type );

        /// <summary>
        /// Conventions for finding an identifier.
        /// </summary>
        public static readonly List<ApplyIdentifierConvention> IdentifierConventions = new List<ApplyIdentifierConvention>();

        public static class Cache
        {
            public const string InstanceCacheContextStorageKey = "SlapperAutoMapper.InstanceCache";

            public static readonly ConcurrentDictionary<Type, Dictionary<string, object>> PropertiesAndFields = new ConcurrentDictionary<Type, Dictionary<string, object>>();

            public static readonly List<TypeMap> TypeMaps = new List<TypeMap>();

            public class TypeMap
            {
                public TypeMap( Type type, IEnumerable<string> identifiers )
                {
                    Type = type;
                    Identifiers = identifiers;
                }

                public readonly Type Type;

                public IEnumerable<string> Identifiers;
            }

            /// <summary>
            /// Clears all internal caches
            /// </summary>
            public static void ClearAllCaches()
            {
                PropertiesAndFields.Clear();
                TypeMaps.Clear();
                ClearInstanceCache();
            }

            /// <summary>
            /// Clears the instance cache. This cache contains all objects created by SlapperAutoMapper.
            /// </summary>
            public static void ClearInstanceCache()
            {
                var instanceCache = CallContext.LogicalGetData( InstanceCacheContextStorageKey ) as Dictionary<object, object>;

                instanceCache = null;

                CallContext.FreeNamedDataSlot( InstanceCacheContextStorageKey );
            }

            /// <summary>
            /// Gets the instance cache containing all objects created by SlapperAutoMapper.
            /// </summary>
            /// <returns></returns>
            public static Dictionary<object, object> GetInstanceCache()
            {
                var instanceCache = CallContext.LogicalGetData( InstanceCacheContextStorageKey ) as Dictionary<object, object>;

                if ( instanceCache == null )
                {
                    instanceCache = new Dictionary<object, object>();

                    CallContext.LogicalSetData( Cache.InstanceCacheContextStorageKey, instanceCache );
                }

                return instanceCache;
            }
        }

        /// <summary>
        /// Applies default conventions for finding identifiers
        /// </summary>
        public static void ApplyDefaultIdentifierConventions()
        {
            IdentifierConventions.Add( type => "Id" );
            IdentifierConventions.Add( type => type.Name + "Id" );
            IdentifierConventions.Add( type => type.Name + "Nbr" );
        }

        /// <summary>
        /// Adds an identifier for the specified type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="identifier">Identifier</param>
        public static void AddIdentifier( Type type, string identifier )
        {
            AddIdentifiers( type, new List<string> { identifier } );
        }

        /// <summary>
        /// Adds identifiers for the specified type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="identifiers">Identifiers</param>
        public static void AddIdentifiers( Type type, IEnumerable<string> identifiers )
        {
            var typeMap = Cache.TypeMaps.FirstOrDefault( x => x.Type == type );

            if ( typeMap != null )
            {
                typeMap.Identifiers = identifiers;
            }
            else
            {
                Cache.TypeMaps.Add( new Cache.TypeMap( type, identifiers ) );
            }
        }

        /// <summary>
        /// Attempts to get the identifiers for the given type. Returns NULL if not found.
        /// Results are cached for subsequent use and performance.
        /// </summary>
        /// <remarks>
        /// If no identifiers have been manually added, this method will attempt
        /// to first find an <see cref="SlapperAutoMapper.Id"/> attribute on the <paramref name="type"/>
        /// and if not found will then try to match based upon any specified identifier conventions.
        /// </remarks>
        /// <param name="type">Type</param>
        /// <returns>Identifier</returns>
        public static IEnumerable<string> TryToGetIdentifiers( Type type )
        {
            var typeMap = Cache.TypeMaps.FirstOrDefault( x => x.Type == type );

            if ( typeMap != null ) return typeMap.Identifiers;

            var conventionIdentifiers = IdentifierConventions.Select( applyIdentifierConvention => applyIdentifierConvention( type ) ).ToList();

            var fieldsAndProperties = GetFieldsAndProperties( type );

            var identifiers = new List<string>();

            foreach ( var fieldOrProperty in fieldsAndProperties )
            {
                var memberName = fieldOrProperty.Key;

                var member = fieldOrProperty.Value;

                if ( member is FieldInfo )
                {
                    if ( ( ( FieldInfo ) member ).GetCustomAttributes( typeof( Id ), false ).Length > 0 )
                    {
                        identifiers.Add( memberName );
                    }
                    else if ( conventionIdentifiers.Exists( x => x.ToLower() == memberName.ToLower() ) )
                    {
                        identifiers.Add( memberName );
                    }
                }
                else if ( member is PropertyInfo )
                {
                    if ( ( ( PropertyInfo ) member ).GetCustomAttributes( typeof( Id ), false ).Length > 0 )
                    {
                        identifiers.Add( memberName );
                    }
                    else if ( conventionIdentifiers.Exists( x => x.ToLower() == memberName.ToLower() ) )
                    {
                        identifiers.Add( memberName );
                    }
                }
            }

            Cache.TypeMaps.Add( new Cache.TypeMap( type, identifiers ) );

            return identifiers.Count == 0 ? null : identifiers;
        }

        /// <summary>
        /// Get a Dictionary of a type's property names and field names and their corresponding PropertyInfo or FieldInfo.
        /// Results are cached for subsequent use and performance.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Dictionary of a type's property names and their corresponding PropertyInfo</returns>
        public static Dictionary<string, object> GetFieldsAndProperties( Type type )
        {
            return Cache.PropertiesAndFields.GetOrAdd( type, CreateFieldAndPropertyInfoDictionary( type ) );
        }

        private static Dictionary<string, object> CreateFieldAndPropertyInfoDictionary( Type type )
        {
            var dictionary = new Dictionary<string, object>();

            var properties = type.GetProperties();

            foreach ( var propertyInfo in properties )
            {
                dictionary.Add( propertyInfo.Name, propertyInfo );
            }

            var fields = type.GetFields();

            foreach ( var fieldInfo in fields )
            {
                dictionary.Add( fieldInfo.Name, fieldInfo );
            }

            return dictionary;
        }

        /// <summary>
        /// Converts a list of dynamic property names and values to a list of type of <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and automap to</typeparam>
        /// <param name="dynamicObject">Dynamic list of property names and values</param>
        /// <returns>List of type <typeparamref name="T"/></returns>
        public static T MapDynamic<T>( object dynamicObject )
        {
            var dictionary = dynamicObject as IDictionary<string, object>;

            if ( dictionary == null )
                throw new ArgumentException( "Object type cannot be converted to an IDictionary<string,object>", "dynamicObject" );

            var propertiesList = new List<IDictionary<string, object>> { dictionary };

            return Map<T>( propertiesList ).FirstOrDefault();
        }

        /// <summary>
        /// Converts a list of dynamic property names and values to a list of type of <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and automap to</typeparam>
        /// <param name="dynamicListOfProperties">Dynamic list of property names and values</param>
        /// <returns>List of type <typeparamref name="T"/></returns>
        public static IEnumerable<T> MapDynamic<T>( IEnumerable<object> dynamicListOfProperties )
        {
            var dictionary = dynamicListOfProperties.Select( dynamicItem => dynamicItem as IDictionary<string, object> ).ToList();

            if ( dictionary == null || dictionary.Count == 0 || dictionary[ 0 ] == null )
                throw new ArgumentException( "Object types cannot be converted to an IDictionary<string,object>", "dynamicListOfProperties" );

            return Map<T>( dictionary );
        }

        /// <summary>
        /// Converts a list of property names and values to a list of type of <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and automap to</typeparam>
        /// <param name="listOfProperties">List of property names and values</param>
        /// <returns>List of type <typeparamref name="T"/></returns>
        public static T Map<T>( IDictionary<string, object> listOfProperties )
        {
            var propertiesList = new List<IDictionary<string, object>> { listOfProperties };

            return Map<T>( propertiesList ).FirstOrDefault();
        }

        /// <summary>
        /// Converts a list of property names and values to a list of type of <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and automap to</typeparam>
        /// <param name="listOfProperties">List of property names and values</param>
        /// <returns>List of type <typeparamref name="T"/></returns>
        public static IEnumerable<T> Map<T>( IEnumerable<IDictionary<string, object>> listOfProperties )
        {
            var instanceCache = new Dictionary<object, object>();

            foreach ( var properties in listOfProperties )
            {
                var getInstanceResult = GetInstance( typeof( T ), properties );

                object instance = getInstanceResult.Item2;

                int instanceIdentifierHash = getInstanceResult.Item3;

                if ( instanceCache.ContainsKey( instanceIdentifierHash ) == false )
                {
                    instanceCache.Add( instanceIdentifierHash, instance );
                }

                var caseInsensitiveDictionary = new Dictionary<string, object>( properties, StringComparer.OrdinalIgnoreCase );

                Map( caseInsensitiveDictionary, instance );
            }

            foreach ( var pair in instanceCache )
            {
                yield return ( T ) pair.Value;
            }
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
        /// <returns>Populated instance</returns>
        public static object Map( IDictionary<string, object> dictionary, object instance )
        {
            var fieldsAndProperties = GetFieldsAndProperties( instance.GetType() );

            foreach ( var fieldOrProperty in fieldsAndProperties )
            {
                var memberName = fieldOrProperty.Key.ToLower();

                var member = fieldOrProperty.Value;

                object value;

                if ( dictionary.TryGetValue( memberName, out value ) )
                {
                    if ( member is FieldInfo )
                    {
                        ( ( FieldInfo ) member ).SetValue( instance, value );
                    }
                    else if ( member is PropertyInfo )
                    {
                        ( ( PropertyInfo ) member ).SetValue( instance, value, null );
                    }
                }
                else
                {
                    Type propertyType = null;

                    if ( member is FieldInfo )
                    {
                        propertyType = ( ( FieldInfo ) member ).FieldType;
                    }
                    else if ( member is PropertyInfo )
                    {
                        propertyType = ( ( PropertyInfo ) member ).PropertyType;
                    }

                    if ( propertyType.IsClass || propertyType.IsInterface )
                    {
                        var nestedDictionary = dictionary.Where( x => x.Key.ToLower().StartsWith( memberName + "_" ) );

                        if ( nestedDictionary.Count() == 0 ) continue;

                        var newDictionary = nestedDictionary.ToDictionary( pair => pair.Key.ToLower().Replace( memberName + "_", string.Empty ), pair => pair.Value, StringComparer.OrdinalIgnoreCase );

                        object nestedInstance = null;

                        if ( member is FieldInfo )
                        {
                            nestedInstance = ( ( FieldInfo ) member ).GetValue( instance );
                        }
                        else if ( member is PropertyInfo )
                        {
                            nestedInstance = ( ( PropertyInfo ) member ).GetValue( instance, null );
                        }

                        if ( nestedInstance == null && propertyType.IsClass )
                        {
                            nestedInstance = Activator.CreateInstance( propertyType );
                        }

                        Type genericCollectionType = typeof( ICollection<> );

                        if ( propertyType.IsGenericType && genericCollectionType.IsAssignableFrom( propertyType.GetGenericTypeDefinition() )
                             || propertyType.GetInterfaces().Any( x => x.IsGenericType && x.GetGenericTypeDefinition() == genericCollectionType ) )
                        {
                            var innerType = propertyType.GetGenericArguments().First();

                            Type baseListType = typeof( List<> );

                            Type listType = baseListType.MakeGenericType( innerType );

                            if ( nestedInstance == null )
                            {
                                nestedInstance = Activator.CreateInstance( listType );
                            }

                            var getInstanceResult = GetInstance( innerType, newDictionary );

                            // Is this a newly created instance? If false, then this item was retrieved from the instance cache.
                            bool isNewlyCreatedInstance = getInstanceResult.Item1;

                            object instanceToAddToCollectionInstance = getInstanceResult.Item2;

                            instanceToAddToCollectionInstance = Map( newDictionary, instanceToAddToCollectionInstance );

                            if ( isNewlyCreatedInstance )
                            {
                                MethodInfo addMethod = listType.GetMethod( "Add" );

                                addMethod.Invoke( nestedInstance, new[] { instanceToAddToCollectionInstance } );
                            }
                            else
                            {
                                MethodInfo containsMethod = listType.GetMethod( "Contains" );

                                bool alreadyContainsInstance = ( bool ) containsMethod.Invoke( nestedInstance, new[] { instanceToAddToCollectionInstance } );

                                if ( alreadyContainsInstance == false )
                                {
                                    MethodInfo addMethod = listType.GetMethod( "Add" );

                                    addMethod.Invoke( nestedInstance, new[] { instanceToAddToCollectionInstance } );
                                }
                            }
                        }
                        else
                        {
                            nestedInstance = Map( newDictionary, nestedInstance );
                        }

                        if ( member is FieldInfo )
                        {
                            ( ( FieldInfo ) member ).SetValue( instance, nestedInstance );
                        }
                        else if ( member is PropertyInfo )
                        {
                            ( ( PropertyInfo ) member ).SetValue( instance, nestedInstance, null );
                        }
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// Gets a new or existing instance depending on whether an instance with the same identifiers already existing
        /// in the instance cache.
        /// </summary>
        /// <param name="type">Type of instance to get</param>
        /// <param name="properties">List of properties and values</param>
        /// <returns>
        /// Tuple of bool, object, int where bool represents whether this is a newly created instance,
        /// object being an instance of the requested type and int being the instance's identifier hash.
        /// </returns>
        private static Tuple<bool, object, int> GetInstance( Type type, IDictionary<string, object> properties )
        {
            var instanceCache = Cache.GetInstanceCache();

            var identifiers = TryToGetIdentifiers( type );

            object instance = null;
            bool isNewlyCreatedInstance = false;

            int identifierHash = 0;

            if ( identifiers != null )
            {
                foreach ( var identifier in identifiers )
                {
                    if ( properties.ContainsKey( identifier ) )
                    {
                        var identifierValue = properties[ identifier ];

                        identifierHash += identifierValue.GetHashCode() + type.GetHashCode();
                    }
                }

                if ( identifierHash != 0 )
                {
                    if ( instanceCache.ContainsKey( identifierHash ) )
                    {
                        instance = instanceCache[ identifierHash ];
                    }
                    else
                    {
                        instance = Activator.CreateInstance( type );

                        instanceCache.Add( identifierHash, instance );

                        isNewlyCreatedInstance = true;
                    }
                }
            }

            if ( instance == null )
            {
                instance = Activator.CreateInstance( type );

                identifierHash = Guid.NewGuid().GetHashCode();

                instanceCache.Add( identifierHash, instance );

                isNewlyCreatedInstance = true;
            }

            return new Tuple<bool, object, int>( isNewlyCreatedInstance, instance, identifierHash );
        }
    }
}