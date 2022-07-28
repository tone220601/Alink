using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;

namespace RfqWebService
{
    /// <summary>
    /// Summary description for CmsWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CmsWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public string CreateRfq(DataTable RfqDt, DataTable RoutesDt)
        {
            SqlConnection cnn = new SqlConnection(WebConfigurationManager.AppSettings["TestCon"]);
            SqlCommand DA = new SqlCommand("RequestForQuote_aLinkInsert", cnn);
            DA.CommandType = CommandType.StoredProcedure;
            DA.Parameters.AddWithValue("@Title", RfqDt.Rows[0]["Title"]);
            DA.Parameters.AddWithValue("@Forename", RfqDt.Rows[0]["Forename"]);
            DA.Parameters.AddWithValue("@Surname", RfqDt.Rows[0]["Surname"]);
            //
            SqlParameter dateOfBirth = new SqlParameter();
            dateOfBirth.ParameterName = "@DateOfBirth";
            dateOfBirth.DbType = DbType.DateTime;
            dateOfBirth.Direction = ParameterDirection.Input;
            dateOfBirth.Value = RfqDt.Rows[0]["DateOfBirth"];
            DA.Parameters.Add(dateOfBirth);


            SqlParameter gender = new SqlParameter();
            gender.ParameterName = "@Gender";
            gender.DbType = DbType.Int32;
            gender.Direction = ParameterDirection.Input;
            gender.Value = RfqDt.Rows[0]["Gender"];
            DA.Parameters.Add(gender);

            DA.Parameters.AddWithValue("@Telephone", RfqDt.Rows[0]["Telephone"]);
            DA.Parameters.AddWithValue("@Email", RfqDt.Rows[0]["Email"]);
            DA.Parameters.AddWithValue("@HouseNameNumber", RfqDt.Rows[0]["HouseNameNumber"]);
            DA.Parameters.AddWithValue("@BuildingName", RfqDt.Rows[0]["BuildingName"]);
            DA.Parameters.AddWithValue("@Street", RfqDt.Rows[0]["Street"]);
            DA.Parameters.AddWithValue("@District", RfqDt.Rows[0]["District"]);
            DA.Parameters.AddWithValue("@Town", RfqDt.Rows[0]["Town"]);
            DA.Parameters.AddWithValue("@County", RfqDt.Rows[0]["County"]);
            DA.Parameters.AddWithValue("@Postcode", RfqDt.Rows[0]["Postcode"]);
            //int
            SqlParameter emplyerid = new SqlParameter();
            emplyerid.ParameterName = "@EmployerId";
            emplyerid.DbType = DbType.Int32;
            emplyerid.Direction = ParameterDirection.Input;
            emplyerid.Value = RfqDt.Rows[0]["EmployerId"];
            DA.Parameters.Add(emplyerid);

            DA.Parameters.AddWithValue("@EmployerNumber", RfqDt.Rows[0]["EmployerNumber"]);
            DA.Parameters.AddWithValue("@RouteTableType", RoutesDt);

            SqlParameter dataSource = new SqlParameter();
            dataSource.ParameterName = "@DataSource";
            dataSource.DbType = DbType.Int32;
            dataSource.Direction = ParameterDirection.Input;
            dataSource.Value = 4;
            DA.Parameters.Add(dataSource);

            SqlParameter returnValue = new SqlParameter();
            returnValue.ParameterName = "@Barcode";
            returnValue.DbType = DbType.String;
            returnValue.Size = 10;
            returnValue.Value = "";
            returnValue.Direction = ParameterDirection.InputOutput;

            DA.Parameters.Add(returnValue);
            DA.Connection.Open();
            DA.ExecuteNonQuery();
            string barcode = returnValue.Value.ToString();
            DA.Dispose();
            return barcode;
        }

        [WebMethod]
        public string testcon()
        {
            SqlDataAdapter DA = new SqlDataAdapter("Select top (1) Forename from  RequestForQuote", WebConfigurationManager.AppSettings["TestCon"]);
            DA.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            DA.Fill(dt);

            return dt.Rows[0][0].ToString();
        }
    }
}
