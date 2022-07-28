using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using System.Xml.Serialization;
using ToolsWebService.Model;

namespace ToolsWebService
{
    /// <summary>
    /// Summary description for NIRFromToWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class NIRFromToWebService : System.Web.Services.WebService
    {
        [WebMethod]
        public List<Stations> GetRailStations()
        {

            var stations = new List<Stations>();
            using (var cnn = new SqlConnection(WebConfigurationManager.AppSettings["TestCon"]))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("Select szLongtext, StationId From lkpRailStations order by szLongtext asc", cnn);
                cmd.CommandType = CommandType.Text;
                var reader= cmd.ExecuteReader();
                while (reader.Read())
                {
                    stations.Add(new Stations() { StationId = int.Parse(reader["StationId"].ToString()), StationName = reader["szLongtext"].ToString() }); 
                }
            }
            //var stationList = Serialize(stations);

            return stations;
        }


        public string Serialize(object obj)
        {
            using (var memoryStream = new MemoryStream())
            using (var reader = new StreamReader(memoryStream))
            {
                var serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        public string HelloWorld()
        {
            return "Hello World";
        }
    }
}
