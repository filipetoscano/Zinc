using Platinum;
using System;
using System.Globalization;

namespace Zinc.WebServices.RestClient
{
    /// <summary />
    public class ServiceFaultException : ActorException
    {
        private string _actor;
        private int _code;


        /// <summary />
        public ServiceFaultException( string actor, int code, string message, ActorException innerException )
            : base( message, innerException )
        {
            #region Validations

            if ( actor == null )
                throw new ArgumentNullException( nameof( actor ) );

            #endregion

            _actor = actor;
            _code = code;
        }


        /// <summary />
        public override string Actor
        {
            get { return _actor; }
        }


        /// <summary />
        public override int Code
        {
            get { return _code; }
        }


        /// <summary />
        public override string Description
        {
            get { return this.Message; }
        }


        /// <summary>
        /// Override the default implementation of .ToString(), so that all
        /// relevant information is available in the string representation.
        /// </summary>
        /// <returns>String representation of error.</returns>
        public override string ToString()
        {
            string s = string.Format( CultureInfo.InvariantCulture, "({0}/{1}) {2}", this.Actor, this.Code, this.Message );

            if ( this.StackTrace != null )
                s = s + "\n" + this.StackTrace;

            if ( this.InnerException != null )
                s = s + "\n\n" + this.InnerException.ToString();

            return s;
        }
    }
}
