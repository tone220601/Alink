using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class RouteOfTravel
    {
        [Key]
        public int RouteOfTravelId { get; set; }

        public int ModeOfTransportId { get; set; }
        public virtual ModeOfTransport ModeOfTravel { get; set; }
        public string RouteAddressFrom { get; set; }
        public string RouteAddressTo { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual RequestForQuotation RequestForQuotation { get; set; }
    }
}