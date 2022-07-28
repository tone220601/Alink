using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using Domain.General;

namespace ToolsWebService.Model
{
    [Serializable]
    public class Stations/* : Entity*/
    {
        public int StationId { get; set; }
        public string StationName { get; set; }
    }
}