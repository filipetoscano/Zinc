using NLog;

namespace Zinc.WebServices.ProxyGenerator
{
    /// <summary />
    public class CommandLine
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        /// <summary />
        public string Assembly { get; set; }

        /// <summary />
        public string Application { get; set; }

        /// <summary />
        public string Namespace { get; set; }

        /// <summary />
        public string Language { get; set; } = "CSharp";

        /// <summary />
        public string Output { get; set; }

        /// <summary />
        public bool Async { get; set; } = true;

        /// <summary />
        public bool Sync { get; set; } = false;


        /// <summary />
        public static CommandLine Parse( string[] args )
        {
            /*
             * Very crappy argument line parsing :p
             */
            CommandLine cl = new CommandLine();

            foreach ( string arg in args )
            {
                if ( arg.StartsWith( "--assembly=" ) == true )
                {
                    cl.Assembly = arg.Substring( "--assembly=".Length );
                    continue;
                }

                if ( arg.StartsWith( "--application=" ) == true )
                {
                    cl.Application = arg.Substring( "--application=".Length );
                    continue;
                }

                if ( arg.StartsWith( "--namespace=" ) == true )
                {
                    cl.Namespace = arg.Substring( "--namespace=".Length );
                    continue;
                }

                if ( arg.StartsWith( "--output=" ) == true )
                {
                    cl.Output = arg.Substring( "--output=".Length );
                    continue;
                }

                if ( arg == "--sync" )
                {
                    cl.Sync = true;
                    continue;
                }

                if ( arg == "--no-async" )
                {
                    cl.Async = false;
                    continue;
                }
            }


            /*
             * 
             */
            if ( string.IsNullOrEmpty( cl.Assembly ) == true )
            {
                logger.Fatal( "err: Assembly is a required option" );
                return null;
            }

            if ( string.IsNullOrEmpty( cl.Application ) == true )
            {
                logger.Fatal( "err: Application is a required option." );
                return null;
            }

            if ( string.IsNullOrEmpty( cl.Namespace ) == true )
            {
                logger.Fatal( "err: Namespace is a required option." );
                return null;
            }

            if ( string.IsNullOrEmpty( cl.Output ) == true )
            {
                logger.Fatal( "err: Output is a required option." );
                return null;
            }

            return cl;
        }
    }
}
