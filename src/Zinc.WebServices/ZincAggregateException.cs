using Platinum;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Zinc.WebServices
{
    /// <summary />
    [Serializable]
    public class ZincAggregateException : ActorException
    {
        private ActorException _leading;


        /// <summary />
        public ZincAggregateException( ActorException leading, ActorAggregateException aggregateException )
            : base( leading?.Message, aggregateException )
        {
            #region Validations

            if ( leading == null )
                throw new ArgumentNullException( nameof( leading ) );

            if ( aggregateException == null )
                throw new ArgumentNullException( nameof( aggregateException ) );

            #endregion

            _leading = leading;
        }



        /// <summary />
        public override string Actor
        {
            get
            {
                return _leading.Actor;
            }
        }


        /// <summary />
        public override int Code
        {
            get
            {
                return _leading.Code;
            }
        }


        /// <summary />
        public override string Description
        {
            get
            {
                var agg = this.InnerException as ActorAggregateException;

                StringBuilder sb = new StringBuilder();
                int x = 0;

                foreach ( ActorException ex in agg )
                {
                    sb.AppendLine( $"#{ x++ }: { ex.Actor }/{ ex.Code }: { ex.Description }" );
                }

                return sb.ToString();
            }
        }


        /// <summary />
        protected ZincAggregateException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
            #region Validations

            if ( info == null )
                throw new ArgumentNullException( nameof( info ) );

            #endregion

            _leading = (ActorException) info.GetValue( "Leading", typeof( ActorException ) );
        }


        /// <summary />
        [SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]
        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            #region Validations

            if ( info == null )
                throw new ArgumentNullException( nameof( info ) );

            #endregion

            info.AddValue( "Leading", _leading );

            base.GetObjectData( info, context );
        }
    }
}
