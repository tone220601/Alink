using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Models
{
    public class Confirmation
    {
        //customer
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }

        public string Town { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Dob { get; set; }
        public int Gender { get; set; }
        //journey

        public List<SelectListItem> ModesOfTransports { get; set; }
        public string ModeOfTravel1 { get; set; }
        public string From1 { get; set; }
        public string To1 { get; set; }
        public string ModeOfTravel2 { get; set; }
        public string From2 { get; set; }
        public string To2 { get; set; }
        public string ModeOfTravel3 { get; set; }
        public string From3 { get; set; }
        public string To3 { get; set; }

        //employer

        public string Employer { get; set; }
        public string EmplyerAddress { get; set; }
        public string EmplyeeNumber { get; set; }
    }
}