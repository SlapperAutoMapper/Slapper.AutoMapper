using System.Linq;
using System.Reflection;

namespace Slapper.Tests
{
    public static class TestHelpers
    {
        /// <summary>
        /// Gets the hash code slapper would create for a particular object.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static int GetHashCode( object instance )
        {
            var instanceType = instance.GetType();

            var identifiers = AutoMapper.InternalHelpers.GetIdentifiers( instanceType );

            int identifierHash = 0;

            if ( identifiers != null )
            {
                var fieldsAndProperties = AutoMapper.InternalHelpers.GetFieldsAndProperties( instanceType );

                foreach ( var fieldOrProperty in fieldsAndProperties )
                {
                    var memberName = fieldOrProperty.Key;

                    if ( identifiers.Contains( memberName ) )
                    {
                        var member = fieldOrProperty.Value;

                        object value = null;

                        if ( member is FieldInfo )
                        {
                            value = ( ( FieldInfo ) member ).GetValue( instance );
                        }
                        else if ( member is PropertyInfo )
                        {
                            value = ( ( PropertyInfo ) member ).GetValue( instance, null );
                        }

                        if ( value != null )
                        {
                            identifierHash += value.GetHashCode() + instanceType.GetHashCode();
                        }
                    }
                }
            }

            return identifierHash;
        }
    }
}