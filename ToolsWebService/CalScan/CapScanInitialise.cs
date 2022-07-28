using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToolsWebService.CalScan
{
    public class CapScanInitialise
    {
        public CapScanInterface Initialise()
        {
            const string county = "COUNTY";

            const string hostName = "192.168.100.160";
            const string poolName = "PAF";
            if (string.IsNullOrEmpty(hostName) || string.IsNullOrEmpty(poolName))
                return null;

            var cs = new CapScanInterface(hostName, poolName);

            cs.SetCounty(county);

            return cs;
        }
    }
}