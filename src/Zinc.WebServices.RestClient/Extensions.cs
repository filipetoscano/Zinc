using System;

namespace Zinc.WebServices.RestClient
{
    /// <summary />
    internal static class Extensions
    {
        /// <summary />
        internal static string EnsureEndsWith( this string str, string value )
        {
            #region Validations

            if ( str == null )
                throw new ArgumentNullException( nameof( str ) );

            if ( value == null )
                throw new ArgumentNullException( nameof( value ) );

            #endregion

            if ( str.EndsWith( value ) == false )
                return str + value;

            return str;
        }


        /// <summary />
        internal static string StripSuffix( this string str, string suffix )
        {
            #region Validations

            if ( str == null )
                throw new ArgumentNullException( nameof( str ) );

            if ( suffix == null )
                throw new ArgumentNullException( nameof( suffix ) );

            #endregion

            if ( str.EndsWith( suffix ) == true )
                return str.Substring( 0, str.Length - suffix.Length );

            return str;
        }
    }
}
