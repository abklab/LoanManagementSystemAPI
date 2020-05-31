using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Proviso_1_2.Models
{
    [DataContract(Namespace = "Proviso_1_2")]
   public class MomoTransaction
    {
        public int EntryID { get; set; }

        [DataMember]
        [Required]
        public string TYPE { get; set; }
        [DataMember]
        [Required]
        public string TXNID { get; set; }
        [DataMember]
        [Required]
        public string MSISDN { get; set; }
        [DataMember]
        [Required]
        public decimal AMOUNT { get; set; }
        [DataMember]
        public string COMPANYNAME { get; set; }
        [DataMember]
        [Required]
        public string CUSTOMERREFERENCEID { get; set; }

        public string RefID
        {
            get
            {
                string datepart = DateTime.Now.ToString("MMdd-yyyy-hhmmss");
                string momoLast4 = MSISDN.Substring(MSISDN.Length - 4);
                string refid = $"{datepart}-{momoLast4}";
                return refid;
            }
        }

        public string MNO { get; set; } = "MNO";

    }
}