using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class Employer
    {

        public Employer()
        {
            RequestForQuotation = new LinkedList<RequestForQuotation>();
        }

        [Key]
        public int EmployerId { get; set; }
        public int CustomerId { get; set; }
        public string EmployerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string PostCode { get; set; }
        public string EmployeeNumber { get; set; }


        public virtual Customer Customer { get; set; }

        public virtual ICollection<RequestForQuotation> RequestForQuotation { get; set; }
    }
}