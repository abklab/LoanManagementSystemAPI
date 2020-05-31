using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace LoanManagementSystemAPI.Response
{
    [DataContract(Namespace = "Proviso_1_2", Name = "COMMAND")]
    public class COMMAND
    {
        [DataMember]
        public string TYPE { get; set; }

        [DataMember]
        public string TXNID { get; set; }
        [DataMember]
        public string RefID { get; set; }
        [DataMember]
        public string MSISDN { get; set; }
        [DataMember]
        public string RESULT { get; set; }
        [DataMember]
        public string ERRORCODE { get; set; }
        [DataMember]
        public string ERRORDESCRIPTION { get; set; }
        [DataMember]
        public string FLAG { get; set; }
        [DataMember]
        public string CONTENT { get; set;}
    }
}