using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class RequestForQuotation
    {
        [Key, Column(Order = 0)]
        public int RouteOfTravelId { get; set; }
        [Column(Order = 1)]
        public int? EmployerId { get; set; }
        [Column(Order = 2)]
        public DateTime RequestDate { get; set; }
        [Column(Order = 3)]
        [MaxLength(10)]
        public string Barcode { get; set; }

        public virtual Employer Employer { get; set; }
        public virtual RouteOfTravel RouteOfTravel { get; set; } 
    }
}