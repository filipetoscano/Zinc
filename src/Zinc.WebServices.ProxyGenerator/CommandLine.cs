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
        public string Module { get; set; }

        /// <summary />
        public string Namespace { get; set; }

        /// <summary />
        public string Language { get; set; } = "CSharp";

        /// <summary />
        public string Output { get; set; }


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

                if ( arg.StartsWith( "--module=" ) == true )
                {
                    cl.Module = arg.Substring( "--module=".Length );
                    continue;
                }

                if ( arg.StartsWith( "--namespace=" ) == true )
                {
                    cl.Namespace = arg.Substring( "--namespace=".Length );
                    continue;
                }

                if ( arg.StartsWith( "--out=" ) == true )
                {
                    cl.Output = arg.Substring( "--out=".Length );
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

            if ( string.IsNullOrEmpty( cl.Module ) == true )
            {
                logger.Fatal( "err: Module is a required option." );
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
