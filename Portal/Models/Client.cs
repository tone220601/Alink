using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class Client
    {
        public Client()
        {
            ClientSchemes = new List<ClientSchemes>();
        }

        [Key]
        public int ClientId { get; set; }
        public string ClientName { get; set; }

        public virtual ICollection<ClientSchemes> ClientSchemes { get; set; }
    }
}