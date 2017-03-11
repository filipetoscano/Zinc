using Platinum;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Zinc.WebServices
{
    /// <summary>
    /// Describes a server-side error, including all properties of <see cref="ActorException" />
    /// but without deriving from <see cref="Exception" />.
    /// </summary>
    /// <remarks>
    /// Custom faults never use the XmlSerializer, even if the interface/method uses it.
    /// As such, in order to place the faults in the right namespace we need to make use
    /// of [DataContract] attribute *and* mark all elements which make part of the
    /// message with [DataMember].
    /// See: http://stackoverflow.com/questions/3246318/custom-soap-fault-has-wrong-namespace-http-schemas-datacontract-org-2004-07
    /// </remarks>
    [DataContract( Name = "ActorFault", Namespace = Zn.Namespace )]
    [XmlType( TypeName = "ActorError", Namespace = Zn.Namespace )]
    [XmlRoot( Namespace = Zn.Namespace )]
    public class ActorFault
    {
        /// <summary>
        /// Gets or sets the Actor.
        /// </summary>
        [DataMember( Order = 0 )]
        public string Actor
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the Code.
        /// </summary>
        [DataMember( Order = 1 )]
        public int Code
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the Message.
        /// </summary>
        [DataMember( Order = 2 )]
        public string Message
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the type of the exception which was thrown.
        /// </summary>
        [DataMember( Order = 3 )]
        public string ExceptionType
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the list of faults that the fault wraps.
        /// </summary>
        [DataMember( Order = 4 )]
        public ActorFault[] InnerFaults
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        [DataMember( Order = 5 )]
        public string StackTrace
        {
            get;
            set;
        }


        /// <summary>
        /// Creates an instance of <see cref="ActorFault"/> from an unhandled exception.
        /// </summary>
        /// <param name="exception">Unhandled exception.</param>
        /// <returns>Instance of <see cref="ActorFault"/>.</returns>
        public static ActorFault FromUnhandled( Exception exception )
        {
            #region Validations

            if ( exception == null )
                throw new ArgumentNullException( "exception" );

            #endregion

            ActorFault fault = new ActorFault();
            fault.Actor = App.Name;
            fault.Code = 990;
            fault.Message = "Unhandled exception.";
            fault.InnerFaults = Walk( exception );

            return fault;
        }


        /// <summary>
        /// Creates an instance of <see cref="ActorFault"/> from an actor exception.
        /// </summary>
        /// <param name="exception">Unhandled exception.</param>
        /// <returns>Instance of <see cref="ActorFault"/>.</returns>
        public static ActorFault From( ActorException exception )
        {
            #region Validations

            if ( exception == null )
                throw new ArgumentNullException( "exception" );

            #endregion

            ActorFault fault = new ActorFault();
            fault.Actor = exception.Actor;
            fault.Code = exception.Code;
            fault.Message = exception.Description;
            fault.ExceptionType = exception.GetType().FullName;
            fault.StackTrace = exception.StackTrace;
            fault.InnerFaults = Walk( exception.InnerException );

            return fault;
        }


        private static ActorFault[] Walk( Exception exception )
        {
            if ( exception == null )
                return null;


            /*
             * AggregateException, which contains Exception[]
             */
            if ( exception is AggregateException )
            {
                AggregateException aex = (AggregateException) exception;
                List<ActorFault> l = new List<ActorFault>();

                foreach ( Exception iex in aex.InnerExceptions )
                {
                    l.AddRange( Walk( iex ) );
                }

                return l.ToArray();
            }


            /*
             * ActorAggregateException, which contains ActorException[]
             */
            if ( exception is ActorAggregateException )
            {
                ActorAggregateException aex = (ActorAggregateException) exception;
                List<ActorFault> l = new List<ActorFault>();

                foreach ( ActorException iex in aex )
                {
                    l.AddRange( Walk( iex ) );
                }

                return l.ToArray();
            }


            /*
             * ActorException
             */
            if ( exception is ActorException )
            {
                ActorException aex = (ActorException) exception;

                ActorFault f = new ActorFault();
                f.Actor = aex.Actor;
                f.Code = aex.Code;
                f.Message = aex.Description;
                f.ExceptionType = aex.GetType().FullName;
                f.StackTrace = aex.StackTrace;
                f.InnerFaults = Walk( aex.InnerException );

                return new ActorFault[] { f };
            }


            /*
             * Exception
             */
            if ( true )
            {
                ActorFault f = new ActorFault();
                f.Actor = App.Name;
                f.Code = 991;
                f.Message = exception.Message;
                f.ExceptionType = exception.GetType().FullName;
                f.StackTrace = exception.StackTrace;
                f.InnerFaults = Walk( exception.InnerException );

                return new ActorFault[] { f };
            }

            // Impossible! :)
        }
    }
}
