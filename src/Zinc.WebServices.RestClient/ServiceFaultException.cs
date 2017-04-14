using Platinum;
using System;

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
    }
}
