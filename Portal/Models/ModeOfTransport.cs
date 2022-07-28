using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class ModeOfTransport
    {
        public ModeOfTransport()
        {
            RouteOfTravel = new List<RouteOfTravel>();
        }

        [Key, Column(Order = 0)]
        public int ModeOfTransportId { get; set; }
        public int ClientSchemeId { get; set; }

        public string ModeOfTransportType { get; set; }

        public virtual ICollection<RouteOfTravel> RouteOfTravel { get; set; }
        public virtual ClientSchemes ClientScheme { get; set; }
    }
}