using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Models
{
    public class Journey
    {
        public List<SelectListItem> ModesOfTransports { get; set; }

        [Required, RegularExpression("^((?!Please Select).)*$", ErrorMessage = "You must select a route!")]
        public int ModeOfTransport1 { get; set; }
        [Required(ErrorMessage = "A valid route From is required")]
        public string From1 { get; set; }
        [Required(ErrorMessage = "A valid route To is required")]
        public string To1 { get; set; }

        public int ModeOfTransport2 { get; set; }
        public string From2 { get; set; }
        public string To2 { get; set; }

        public int ModeOfTransport3 { get; set; }
        public string From3 { get; set; }
        public string To3 { get; set; }
    }
}