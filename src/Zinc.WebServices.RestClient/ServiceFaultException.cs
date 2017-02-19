using Platinum;
using System;
using System.Globalization;

namespace Zinc.WebServices.RestClient
{
    /// <summary />
    public class ServiceFaultException : ActorException
    {
        /// <summary />
        public ServiceFaultException( string url, ServiceFault fault )
            : base( fault.Message )
        {
            #region Validations

            if ( url == null )
                throw new ArgumentNullException( nameof( url ) );

            if ( fault == null )
                throw new ArgumentNullException( nameof( fault ) );

            #endregion

            this.ServiceUrl = url;
            this.Fault = fault;
        }


        /// <summary />
        public string ServiceUrl
        {
            get;
            private set;
        }


        /// <summary />
        public ServiceFault Fault
        {
            get;
            private set;
        }


        /// <summary />
        public override string Actor
        {
            get { return this.Fault.Actor; }
        }


        /// <summary />
        public override int Code
        {
            get { return this.Fault.Code; }
        }


        /// <summary />
        public override string Description
        {
            get { return this.Fault.Message; }
        }


        /// <summary />
        public override string ToString()
        {
            // TODO: Remove override with next version of Platinum

            string s = string.Format( CultureInfo.InvariantCulture, "({0}/{1}) {2} [{3}]", this.Actor, this.Code, this.Description, this.Message );

            if ( this.StackTrace != null )
                s = s + "\n" + this.StackTrace;

            if ( this.InnerException != null )
                s = s + "\n\n" + this.InnerException.ToString();

            return s;
        }
    }
}
