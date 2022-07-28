using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class EditDetails
    {
        public Customer Customer { get; set; }
        public Employer Employer { get; set; }
        public Journey Journey { get; set; }
    }
}