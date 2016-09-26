using System.Collections.Generic;

namespace Zinc.WebService.ServiceUtil
{
    /// <summary />
    public class CommandLine
    {
        public CommandLine()
        {
            InputFiles = new List<string>();
        }

        /// <summary>
        /// Name of all of the input files from a single Zinc application.
        /// </summary>
        public List<string> InputFiles { get; set; }

        /// <summary>
        /// Name of the consolidated output file.
        /// </summary>
        public string OutputFile { get; set; }

        /// <summary>
        /// Whether to consume/delete the input files, once the tool
        /// successfully completes.
        /// </summary>
        public bool InputDelete { get; set; }

        /// <summary>
        /// Whether to generate a proxy which uses the SOAP interface.
        /// </summary>
        public bool Soap { get; set; }

        /// <summary>
        /// Whether to generate a proxy which uses the REST interface.
        /// </summary>
        public bool Rest { get; set; }

        /// <summary />
        public bool ShrinkAttributes { get; set; }

        /// <summary />
        public bool ShrinkNamespaces { get; set; }

        /// <summary />
        public bool ShrinkProperties { get; set; }

        /// <summary />
        public bool HasShrink
        {
            get
            {
                if ( ShrinkAttributes == true )
                    return true;

                if ( ShrinkNamespaces == true )
                    return true;

                if ( ShrinkProperties == true )
                    return true;

                return false;
            }
        }
    }
}
