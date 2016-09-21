using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Zinc.WebServices
{
    [Serializable]
    [DebuggerStepThrough]
    [DataContractAttribute( Name = "ActorFault", Namespace = Zn.Namespace )]
    [XmlType( TypeName = "ActorError", Namespace = Zn.Namespace )]
    public partial class ActorFault : object, IExtensibleDataObject
    {
        [XmlIgnore]
        public ExtensionDataObject ExtensionData
        {
            get; set;
        }

        [DataMember]
        public string Actor
        {
            get; set;
        }

        [DataMember( Order = 1 )]
        public int Code
        {
            get; set;
        }

        [DataMember( Order = 2 )]
        public string Message
        {
            get; set;
        }

        [DataMember( Order = 3 )]
        public string ExceptionType
        {
            get; set;
        }

        [DataMember( Order = 4 )]
        public ActorFault[] InnerFaults
        {
            get; set;
        }

        [DataMember( Order = 5 )]
        public string StackTrace
        {
            get; set;
        }
    }
}
