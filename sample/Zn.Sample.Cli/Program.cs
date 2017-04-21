using System;
using System.Threading.Tasks;

namespace Zn.Sample.Cli
{
    class Program
    {
        static void Main( string[] args )
        {
            int v = int.Parse( args[ 0 ] );

            /*
             * 
             */
            try
            {
                //Console.WriteLine( "SYNC:" );
                //MainSync( v );

                Console.WriteLine( "ASYNC:" );
                MainAsync( v ).GetAwaiter().GetResult();
            }
            catch ( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }


        public static async Task MainAsync( int value )
        {
            var svc = new OneClient();
            svc.ActivityId = Guid.NewGuid();


            for ( int i = 0; i < 10; i++ )
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
                catch ( Exception )
                {
                    Console.Write( "EX " );
                    //Console.WriteLine( ex.ToString() );
                }

                DateTime end = DateTime.UtcNow;
                TimeSpan ts = end - start;
                Console.WriteLine( "{0} {1:000.000}ms", i, ts.TotalMilliseconds );
            }
        }





        public static void MainSync( int value )
        {
            var svc = new OneClient();
            svc.ActivityId = Guid.NewGuid();


            for ( int i = 0; i < 10; i++ )
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
