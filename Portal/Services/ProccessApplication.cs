using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Portal.Models;

namespace Portal.Services
{
    public class ProccessApplication
    {
        public ProccessApplication()
        {
            CreateTables();
        }

        private DataTable CustomerdDataTable;
        private DataTable RouteDataTable;

        private AlinkContext db = new AlinkContext();

        public string CreateRfq(EditDetails confirmation, int gender)
        {
            var link = new RfqServiceReference.CmsWebServiceSoapClient();
            ProcessRfqData(confirmation, gender);
            return link.CreateRfq(CustomerdDataTable, RouteDataTable);


        }

        private void ProcessRfqData(EditDetails confirmation, int gender)
        {
            CustomerdDataTable.TableName = "Customer";
            CustomerdDataTable.Rows.Add();
            CustomerdDataTable.Rows[0]["Title"] = confirmation.Customer.Title;
            CustomerdDataTable.Rows[0]["Forename"] = confirmation.Customer.FirstName;
            CustomerdDataTable.Rows[0]["Surname"] = confirmation.Customer.Surname;
            CustomerdDataTable.Rows[0]["DateOfBirth"] = confirmation.Customer.DateOfBirth;
            CustomerdDataTable.Rows[0]["Gender"] = gender;
            CustomerdDataTable.Rows[0]["Telephone"] = confirmation.Customer.TelePhoneNumber;
            CustomerdDataTable.Rows[0]["Email"] = confirmation.Customer.Email;
            CustomerdDataTable.Rows[0]["HouseNameNumber"] = confirmation.Customer.Address1;
            CustomerdDataTable.Rows[0]["BuildingName"] = confirmation.Customer.Address2;
            CustomerdDataTable.Rows[0]["Street"] = confirmation.Customer.Address3;
            CustomerdDataTable.Rows[0]["District"] = string.Empty;
            CustomerdDataTable.Rows[0]["town"] = confirmation.Customer.Town;
            CustomerdDataTable.Rows[0]["County"] = confirmation.Customer.County;
            CustomerdDataTable.Rows[0]["Postcode"] = confirmation.Customer.PostCode;
            if (confirmation.Employer != null)
            {
                CustomerdDataTable.Rows[0]["EmployerId"] = confirmation.Employer.EmployerId;
                CustomerdDataTable.Rows[0]["EmployerNumber"] = confirmation.Employer.EmployeeNumber;

            }
            else
            {
                CustomerdDataTable.Rows[0]["EmployerId"] = 0;
                CustomerdDataTable.Rows[0]["EmployerNumber"] = string.Empty;

            }
            int routeid = 1;
            RouteDataTable.TableName = "Route";
            RouteDataTable.Rows.Add();
            RouteDataTable.Rows[0]["Id"] = routeid;
            RouteDataTable.Rows[0]["JourneyId"] = 1;
            RouteDataTable.Rows[0]["routeType"] = confirmation.Journey.ModeOfTransport1;
            RouteDataTable.Rows[0]["From"] = confirmation.Journey.From1;
            RouteDataTable.Rows[0]["To"] = confirmation.Journey.To1;
            RouteDataTable.Rows[0]["Price"] = 0;
            if (confirmation.Journey.ModeOfTransport2 != 4)
            {
                RouteDataTable.Rows.Add();
                RouteDataTable.Rows[1]["Id"] = routeid += 1;
                RouteDataTable.Rows[1]["JourneyId"] = 1;
                RouteDataTable.Rows[1]["routeType"] = confirmation.Journey.ModeOfTransport2;
                RouteDataTable.Rows[1]["From"] = confirmation.Journey.From2;
                RouteDataTable.Rows[1]["To"] = confirmation.Journey.To2;
                RouteDataTable.Rows[1]["Price"] = 0;
                if (confirmation.Journey.ModeOfTransport3 != 4)
                {
                    RouteDataTable.Rows.Add();
                    RouteDataTable.Rows[2]["Id"] = routeid += 1;
                    RouteDataTable.Rows[2]["JourneyId"] = 1;
                    RouteDataTable.Rows[2]["routeType"] = confirmation.Journey.ModeOfTransport3;
                    RouteDataTable.Rows[2]["From"] = confirmation.Journey.From3;
                    RouteDataTable.Rows[2]["To"] = confirmation.Journey.To3;
                    RouteDataTable.Rows[2]["Price"] = 0;
                }
            }
        }

        private void CreateTables()
        {
            CustomerdDataTable = new DataTable();

            CustomerdDataTable.Columns.Add("Title", typeof(string));
            CustomerdDataTable.Columns.Add("Forename", typeof(string));
            CustomerdDataTable.Columns.Add("Surname", typeof(string));
            CustomerdDataTable.Columns.Add("DateOfBirth", typeof(string));
            CustomerdDataTable.Columns.Add("Gender", typeof(int));
            CustomerdDataTable.Columns.Add("Telephone", typeof(string));
            CustomerdDataTable.Columns.Add("Email", typeof(string));
            CustomerdDataTable.Columns.Add("HouseNameNumber", typeof(string));
            CustomerdDataTable.Columns.Add("BuildingName", typeof(string));
            CustomerdDataTable.Columns.Add("Street", typeof(string));
            CustomerdDataTable.Columns.Add("District", typeof(string));
            CustomerdDataTable.Columns.Add("Town", typeof(string));
            CustomerdDataTable.Columns.Add("County", typeof(string));
            CustomerdDataTable.Columns.Add("Postcode", typeof(string));
            CustomerdDataTable.Columns.Add("EmployerId", typeof(int));
            CustomerdDataTable.Columns.Add("EmployerNumber", typeof(string));

            RouteDataTable = new DataTable();
            RouteDataTable.Columns.Add("id", typeof(int));
            RouteDataTable.Columns.Add("JourneyId", typeof(int));
            RouteDataTable.Columns.Add("RouteType", typeof(int));
            RouteDataTable.Columns.Add("From", typeof(string));
            RouteDataTable.Columns.Add("To", typeof(string));
            RouteDataTable.Columns.Add("Price", typeof(string));
        }

       
    }
}