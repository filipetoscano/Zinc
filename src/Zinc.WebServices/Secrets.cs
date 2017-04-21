using System;
using System.Reflection;
using Platinum.Reflection;

namespace Zinc.WebServices
{
    /// <summary />
    internal static class Secrets
    {
        /// <summary />
        internal static T Strip<T>( T value )
        {
            if ( value == null )
                return default( T );

            return (T) StripSecrets( value );
        }


        /// <summary />
        private static object StripSecrets( object value )
        {
            if ( value == null )
                return null;


            /*
             * 
             */
            Type type = value.GetType();

            if ( type.GetCustomAttribute<HasSecretAttribute>() == null )
                return value;


            /*
             * Four cases:
             *  * Class[] with HasSecretAttribute
             *  * Class   with HasSecretAttribute
             *  * prop    with    SecretAttribute
             *  * Everything else :-)
             */
            object t = Activator.CreateInstance( type );

            foreach ( var prop in type.GetProperties() )
            {
                if ( prop.PropertyType.IsArray == true
                    && prop.PropertyType.GetElementType().IsCustomClass() == true
                    && prop.PropertyType.GetElementType().GetCustomAttribute<HasSecretAttribute>() != null )
                {
                    Array v = (Array) prop.GetValue( value );
                    Array m;

                    if ( v == null || v.Length == 0 )
                    {
                        m = v;
                    }
                    else
                    {
                        m = Array.CreateInstance( prop.PropertyType.GetElementType(), v.Length );

                        for ( int i = 0; i < v.Length; i++ )
                        {
                            var vx = v.GetValue( i );
                            var mx = StripSecrets( vx );
                            m.SetValue( mx, i );
                        }
                    }

                    prop.SetValue( t, m );
                }
                else if ( prop.PropertyType.IsCustomClass() == true
                    && prop.PropertyType.GetCustomAttribute<HasSecretAttribute>() != null )
                {
                    var v = StripSecrets( prop.GetValue( value ) );
                    prop.SetValue( t, v );
                }
                else if ( prop.GetCustomAttribute<SecretAttribute>() != null )
                {
                    var v = prop.GetValue( value );

                    if ( v != null )
                    {
                        if ( prop.PropertyType == typeof( string ) )
                            prop.SetValue( t, "ಠ_ಠ secret" );

                        if ( prop.PropertyType == typeof( byte[] ) )
                            prop.SetValue( t, Convert.FromBase64String( "4LKgX+CyoCBzZWNyZXQ=" ) );
                    }
                }
                else
                {
                    prop.SetValue( t, prop.GetValue( value ) );
                }
            }

            return t;
        }
    }
}
