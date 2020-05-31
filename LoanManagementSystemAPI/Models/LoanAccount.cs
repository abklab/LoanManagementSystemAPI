using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proviso_1_2.Models
{
    public class LoanAccount
    {
        public string Account_Number { get; set; }
        public string Account_Name { get; set; }
        public decimal Balance { get; set; }
    }
}