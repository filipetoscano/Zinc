using System;

namespace Zinc.WebServices.ProxyGenerator
{
    /// <summary />
    internal class Konsole
    {
        private static ConsoleColor FatalColor = ConsoleColor.Red;
        private static ConsoleColor WarnColor = ConsoleColor.Yellow;
        private static ConsoleColor InfoColor = ConsoleColor.White;
        private static ConsoleColor DebugColor = ConsoleColor.Gray;


        /// <summary />
        internal static void Fatal( string message )
        {
            Console.ForegroundColor = FatalColor;
            Console.WriteLine( message );
        }


        /// <summary />
        internal static void Fatal( string message, params object[] args )
        {
            Console.ForegroundColor = FatalColor;
            Console.WriteLine( message, args );
        }


        /// <summary />
        internal static void Fatal( Exception exception, string message, params object[] args )
        {
            #region Validations

            if ( exception == null )
                throw new ArgumentNullException( nameof( exception ) );

            #endregion

            Console.ForegroundColor = FatalColor;
            Console.WriteLine( message, args );
            Console.WriteLine( exception.ToString() );
        }


        /// <summary />
        internal static void Fatal( Exception exception )
        {
            #region Validations

            if ( exception == null )
                throw new ArgumentNullException( nameof( exception ) );

            #endregion

            Console.ForegroundColor = FatalColor;
            Console.WriteLine( exception.ToString() );
        }


        /// <summary />
        internal static void Warn( string message )
        {
            Console.ForegroundColor = WarnColor;
            Console.WriteLine( message );
        }


        /// <summary />
        internal static void Warn( string message, params object[] args )
        {
            Console.ForegroundColor = WarnColor;
            Console.WriteLine( message, args );
        }


        /// <summary />
        internal static void Warn( Exception exception, string message, params object[] args )
        {
            #region Validations

            if ( exception == null )
                throw new ArgumentNullException( nameof( exception ) );

            #endregion

            Console.ForegroundColor = WarnColor;
            Console.WriteLine( message, args );
            Console.WriteLine( exception.ToString() );
        }


        /// <summary />
        internal static void Warn( Exception exception )
        {
            #region Validations

            if ( exception == null )
                throw new ArgumentNullException( nameof( exception ) );

            #endregion

            Console.ForegroundColor = WarnColor;
            Console.WriteLine( exception.ToString() );
        }


        /// <summary />
        internal static void Info( string message )
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine( message );
        }


        /// <summary />
        internal static void Info( string message, params object[] args )
        {
            Console.ForegroundColor = InfoColor;
            Console.WriteLine( message, args );
        }


        /// <summary />
        internal static void Info( Exception exception, string message, params object[] args )
        {
            #region Validations

            if ( exception == null )
                throw new ArgumentNullException( nameof( exception ) );

            #endregion

            Console.ForegroundColor = InfoColor;
            Console.WriteLine( message, args );
            Console.WriteLine( exception.ToString() );
        }


        /// <summary />
        internal static void Info( Exception exception )
        {
            #region Validations

            if ( exception == null )
                throw new ArgumentNullException( nameof( exception ) );

            #endregion

            Console.ForegroundColor = InfoColor;
            Console.WriteLine( exception.ToString() );
        }


        /// <summary />
        internal static void Debug( string message )
        {
            Console.ForegroundColor = DebugColor;
            Console.WriteLine( message );
        }


        /// <summary />
        internal static void Debug( string message, params object[] args )
        {
            Console.ForegroundColor = DebugColor;
            Console.WriteLine( message, args );
        }


        /// <summary />
        internal static void Debug( Exception exception, string message, params object[] args )
        {
            #region Validations

            if ( exception == null )
                throw new ArgumentNullException( nameof( exception ) );

            #endregion

            Console.ForegroundColor = DebugColor;
            Console.WriteLine( message, args );
            Console.WriteLine( exception.ToString() );
        }


        /// <summary />
        internal static void Debug( Exception exception )
        {
            #region Validations

            if ( exception == null )
                throw new ArgumentNullException( nameof( exception ) );

            #endregion

            Console.ForegroundColor = DebugColor;
            Console.WriteLine( exception.ToString() );
        }
    }
}
