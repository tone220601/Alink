using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class ClientSchemes
    {
        public ClientSchemes()
        {
            ModeOfTransports = new List<ModeOfTransport>();
        }

        [Key]
        public int ClientTypeId { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
        public string ClientScheme { get; set; }

        public virtual ICollection<ModeOfTransport> ModeOfTransports { get; set; }
    }
}