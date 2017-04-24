using System;
using System.Linq;
using System.Threading.Tasks;
using Zinc.WebServices.RestClient;

namespace Zn.Sample.Cli
{
    /// <summary />
    class Program
    {
        /// <summary />
        static void Main( string[] args )
        {
            int v = 0;
            int n = 10;

            if ( args.Length > 0 )
                v = int.Parse( args[ 0 ] );

            if ( args.Length > 1 )
                n = int.Parse( args[ 1 ] );

            try
            {
                // Console.WriteLine( "SYNC:" );
                // MainSync( v, n );

                Console.WriteLine( "ASYNC" );
                MainAsync( v, n ).GetAwaiter().GetResult();
            }
            catch ( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }


        /// <summary />
        public static async Task MainAsync( int value, int count )
        {
            var svc = new OneClient();
            svc.ActivityId = Guid.NewGuid();

            Console.WriteLine( "URL=" + svc.ServiceUrl );

            for ( int i = 0; i < count; i++ )
            {
                DateTime start = DateTime.UtcNow;

                var t1 = svc.MethodOneAsync( new OneSvc.MethodOneRequest()
                {
                    Value = value,
                } );

                var t2 = svc.MethodOneAsync( new OneSvc.MethodOneRequest()
                {
                    Value = value + 5,
                } );

                var t3 = svc.MethodOneAsync( new OneSvc.MethodOneRequest()
                {
                    Value = value + 3,
                } );


                /*
                 * 
                 */
                try
                {
                    await Task.WhenAll( t1, t2, t3 );
                    Console.Write( "OK " );
                }
                catch ( AggregateException ex )
                {
                    if ( ex.InnerExceptions.All( x => x is ServiceFaultException ) == true )
                        Console.Write( "ER " );
                    else
                        Console.Write( "EX " );
                }
                catch ( ServiceFaultException )
                {
                    Console.Write( "ER " );
                }
                catch ( Exception ex )
                {
                    Console.Write( "EX " );
                    Console.WriteLine( ex.ToString() );
                }

                DateTime end = DateTime.UtcNow;
                TimeSpan ts = end - start;
                Console.WriteLine( "{0} {1:000.000}ms", i, ts.TotalMilliseconds );
            }
        }


        /// <summary />
        public static void MainSync( int value, int count )
        {
            var svc = new OneClient();
            svc.ActivityId = Guid.NewGuid();

            Console.WriteLine( "URL=" + svc.ServiceUrl );

            for ( int i = 0; i < count; i++ )
            {
                DateTime start = DateTime.UtcNow;


                try
                {
                    var t1 = svc.MethodOne( new OneSvc.MethodOneRequest()
                    {
                        Value = value,
                    } );
                    Console.Write( "OK " );
                }
                catch ( ServiceFaultException )
                {
                    Console.Write( "ER " );
                }
                catch ( Exception )
                {
                    Console.Write( "EX " );
                }


                try
                {
                    var t2 = svc.MethodOne( new OneSvc.MethodOneRequest()
                    {
                        Value = value + 5,
                    } );
                    Console.Write( "OK " );
                }
                catch ( ServiceFaultException )
                {
                    Console.Write( "ER " );
                }
                catch ( Exception )
                {
                    Console.Write( "EX " );
                }


                try
                {
                    var t3 = svc.MethodOne( new OneSvc.MethodOneRequest()
                    {
                        Value = value + 3,
                    } );
                    Console.Write( "OK " );
                }
                catch ( ServiceFaultException )
                {
                    Console.Write( "ER " );
                }
                catch ( Exception )
                {
                    Console.Write( "EX " );
                }

                DateTime end = DateTime.UtcNow;
                TimeSpan ts = end - start;
                Console.WriteLine( "{0} {1:000.000}ms", i, ts.TotalMilliseconds );
            }
        }
    }
}
