using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using Domain.Addresses;
using Domain.General.Utils;
using ToolsWebService.CalScan;
using AddressLookupResponse = ToolsWebService.Model.AddressLookupResponse;
namespace ToolsWebService
{
    /// <summary>
    /// Summary description for AddressLookUpWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AddressLookUpWebService : System.Web.Services.WebService
    {
        /// <summary>
        /// Return a list of addresses that match the given postcode and house number using CapScan.
        /// </summary>
        /// <param name="postcode"></param>
        /// <param name="houseNumberOrName"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        [WebMethod]
        public AddressLookupResponse LookupAddresses(string postcode, string houseNumberOrName, int applicationId)
        {
            try
            {
                AddressLookupResponse addressLookupResponse = null;

                if (!string.IsNullOrWhiteSpace(postcode))
                {
                    // Initialise Capscan by setting the server, county and pool
                    var capScanInitialise = new CapScanInitialise();
                    var cs = capScanInitialise.Initialise();

                    // Search for the address using the lookup postcode
                    var search = cs.SearchForAddress(postcode);

                    if (search)
                    {
                        // If a single address was returned...
                        if (cs.AmbiguityDictionary != null && cs.AmbiguityDictionary.Count == 1)
                        {
                            addressLookupResponse = new AddressLookupResponse(GetFullAddress(ref cs));
                        }
                        else
                        {
                            // Multiple addresses where returned. Let's use any provided house number to try to get one address.
                            if (!String.IsNullOrWhiteSpace(houseNumberOrName))
                            {
                                houseNumberOrName += " ";
                                var singleAddressPostcode = (from i in cs.AmbiguityDictionary
                                                             where i.Value.StartsWith(houseNumberOrName)
                                                             select i.Key).SingleOrDefault();
                                addressLookupResponse = singleAddressPostcode != null
                                                            ? new AddressLookupResponse(
                                                              LookupAddress(singleAddressPostcode)) // Single Address Found.
                                                            : new AddressLookupResponse(DataContractSerialization.Serialize(cs.AmbiguityDictionary)); // Not found...return the lot.
                            }
                            else
                            {
                                // No HouseNameOrNumber provided so we'll just have to return the lot.
                                addressLookupResponse = new AddressLookupResponse(DataContractSerialization.Serialize(cs.AmbiguityDictionary));
                            }
                        }
                    }
                }

                return addressLookupResponse;
            }
            catch (Exception ex)
            {
                //LogError("EXCEPTION - Address Lookup", ex, CorrelationType.IncompleteApplication, applicationId);
                return new AddressLookupResponse { Successful = false };
            }
        }

        [WebMethod]
        public bool IsWithinSurrey(string postcode)
        {
            postcode = postcode.ToUpper().Replace(" ", "");
            if (ConfigurationManager.AppSettings["AdditionalAllowedPostcodes"].Split(',').Contains(postcode))
                return true;
            var ward = GetWardDistrict(postcode);
            if (!String.IsNullOrEmpty(ward))
            {
                ward = ward.ToLower();
                return ward == "elmbridge" ||
                       ward == "epsom and ewell" ||
                       ward == "guildford" ||
                       ward == "mole valley" ||
                       ward == "reigate and banstead" ||
                       ward == "runnymede" ||
                       ward == "spelthorne" ||
                       ward == "tandridge" ||
                       ward == "waverley" ||
                       ward == "woking" ||
                       ward == "surrey heath";
            }
            return false;
        }

        private string GetWardDistrict(string postcode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(postcode))
                {
                    // Initialise Capscan by setting the server, county and pool
                    var capScanInitialise = new CapScanInitialise();
                    var cs = capScanInitialise.Initialise();

                    // Search for the address using the chosen address postcode
                    var search = cs.SearchForAddress(postcode);

                    // Check the search has returned some data.
                    if (search)
                    {
                        return cs.GetWardDistrict();
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.Error("EXCEPTION - GetWardDistrict", ex);
            }
            return null;
        }

        /// <summary>
        /// Lookup a specific single address for the given postcode 
        /// using CapScan and return a matched single address.
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        private static Address LookupAddress(string postcode)
        {
            Address address = null;

            if (!String.IsNullOrWhiteSpace(postcode))
            {
                // Initialise Capscan by setting the server, county and pool
                var capScanInitialise = new CapScanInitialise();
                var cs = capScanInitialise.Initialise();

                // Search for the address using the chosen address postcode
                var search = cs.SearchForAddress(postcode);

                // Check the search has returned some data.
                if (search)
                {
                    address = GetFullAddress(ref cs);
                }
            }

            return address;
        }

        /// <summary>
        /// Return single matched address from CapScan.
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        private static Address GetFullAddress(ref CapScanInterface cs)
        {
            Address address = null;

            if (cs != null)
            {
                var rawAddress = new Dictionary<string, string>
                                 {
                                     {"ORGANISATION", cs.GetField("ORGANISATION")},
                                     {"SUBBUILDING", cs.GetField("SUBBUILDING")},
                                     {"BUILDINGNAME", cs.GetField("BUILDINGNAME")},
                                     {"BUILDINGNUMBER", cs.GetField("BUILDINGNUMBER")},
                                     {"DEPSTREET", cs.GetField("DEPSTREET")},
                                     {"STREET", cs.GetField("STREET")},
                                     {"DEPLOCALITY", cs.GetField("DEPLOCALITY")},
                                     {"LOCALITY", cs.GetField("LOCALITY")},
                                     {"POSTTOWN", cs.GetField("POSTTOWN")},
                                     {"COUNTY", cs.GetField("COUNTY")},
                                     {"POSTCODE", cs.GetField("POSTCODE")}
                                 };
                var formattedAddress = FormattedAddressLines(rawAddress);
                address = new Address
                {
                    AddressLine1 = formattedAddress[0],
                    AddressLine2 = formattedAddress[1],
                    AddressLine3 = formattedAddress[2],
                    TownOrCity = formattedAddress[3],
                    County = formattedAddress[4],
                    Postcode = formattedAddress[5]
                };
            }

            return address;
        }

        private static bool LooksLikeANumber(string input, IEnumerable<string> excludeStrings = null)
        {
            // Returns true for '12', '301', '4a', 5/c', '4|b', '3-4', '204-205' etc
            var str = input;
            if (excludeStrings != null)
                str = excludeStrings.Aggregate(str, (current, excludeString) => current.Replace(excludeString, string.Empty));
            return System.Text.RegularExpressions.Regex.IsMatch(str, @"\d") && str.Length <= 10 && (str.Count(char.IsDigit) + str.Count(char.IsPunctuation) + str.Count(char.IsSeparator)) >= str.Count(char.IsLetter);
        }

        private static string GetOrganisation(Dictionary<string, string> rawAddress)
        {
            return string.IsNullOrEmpty(rawAddress["ORGANISATION"]) ? null : rawAddress["ORGANISATION"];
        }

        private static string GetBuilding(Dictionary<string, string> rawAddress)
        {
            if (!string.IsNullOrEmpty(rawAddress["BUILDINGNAME"]))
            {
                if (!string.IsNullOrEmpty(rawAddress["SUBBUILDING"]))
                {
                    return (rawAddress["SUBBUILDING"] + (LooksLikeANumber(rawAddress["SUBBUILDING"]) ? " " : ", ") + rawAddress["BUILDINGNAME"]).Trim();
                }
                else
                {
                    return string.IsNullOrEmpty(rawAddress["BUILDINGNAME"].Trim()) ? null : rawAddress["BUILDINGNAME"].Trim();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(rawAddress["SUBBUILDING"]))
                {
                    return string.IsNullOrEmpty(rawAddress["SUBBUILDING"].Trim()) ? null : rawAddress["SUBBUILDING"].Trim();
                }
                else
                {
                    return null;
                }
            }
        }

        private static string GetStreet(Dictionary<string, string> rawAddress)
        {
            if (!string.IsNullOrEmpty(rawAddress["STREET"]))
            {
                if (!string.IsNullOrEmpty(rawAddress["DEPSTREET"]))
                {
                    if (!string.IsNullOrEmpty(rawAddress["BUILDINGNUMBER"]))
                    {
                        return (rawAddress["BUILDINGNUMBER"] + " " + rawAddress["DEPSTREET"] + ", " + rawAddress["STREET"]).Trim();
                    }
                    else
                    {
                        return (rawAddress["BUILDINGNUMBER"] + " " + rawAddress["DEPSTREET"]).Trim();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(rawAddress["BUILDINGNUMBER"]))
                    {
                        return (rawAddress["BUILDINGNUMBER"] + " " + rawAddress["STREET"]).Trim();
                    }
                    else
                    {
                        return string.IsNullOrEmpty(rawAddress["STREET"].Trim()) ? null : rawAddress["STREET"].Trim();
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(rawAddress["DEPSTREET"]))
                {
                    if (!string.IsNullOrEmpty(rawAddress["BUILDINGNUMBER"]))
                    {
                        return (rawAddress["BUILDINGNUMBER"] + " " + rawAddress["DEPSTREET"]).Trim();
                    }
                    else
                    {
                        return (rawAddress["BUILDINGNUMBER"] + " " + rawAddress["DEPSTREET"]).Trim();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(rawAddress["BUILDINGNUMBER"]))
                    {
                        return string.IsNullOrEmpty(rawAddress["BUILDINGNUMBER"].Trim()) ? null : rawAddress["BUILDINGNUMBER"].Trim();
                    }
                    else
                    {
                        return string.IsNullOrEmpty(rawAddress["STREET"].Trim()) ? null : rawAddress["STREET"].Trim();
                    }
                }
            }
        }

        private static string GetLocality(Dictionary<string, string> rawAddress)
        {
            if (!string.IsNullOrEmpty(rawAddress["LOCALITY"]))
            {
                if (!string.IsNullOrEmpty(rawAddress["DEPLOCALITY"]))
                {
                    return (rawAddress["DEPLOCALITY"] + ", " + rawAddress["LOCALITY"]).Trim();
                }
                else
                {
                    return string.IsNullOrEmpty(rawAddress["LOCALITY"].Trim()) ? null : rawAddress["LOCALITY"].Trim();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(rawAddress["DEPLOCALITY"]))
                {
                    return string.IsNullOrEmpty(rawAddress["DEPLOCALITY"].Trim()) ? null : rawAddress["DEPLOCALITY"].Trim();
                }
                else
                {
                    return null;
                }
            }
        }

        private static string GetPostTown(Dictionary<string, string> rawAddress)
        {
            return rawAddress["POSTTOWN"];
        }

        private static string GetCounty(Dictionary<string, string> rawAddress)
        {
            return rawAddress["COUNTY"];
        }

        private static string GetPostcode(Dictionary<string, string> rawAddress)
        {
            return rawAddress["POSTCODE"];
        }

        private static string[] FormattedAddressLines(Dictionary<string, string> rawAddress)
        {
            var lines = new string[6];
            lines[0] = GetOrganisation(rawAddress) ?? GetBuilding(rawAddress) ?? GetStreet(rawAddress) ?? GetLocality(rawAddress) ?? string.Empty;
            if (GetOrganisation(rawAddress) == null)
            {
                lines[1] = GetStreet(rawAddress) ?? GetLocality(rawAddress) ?? string.Empty;
                lines[2] = GetLocality(rawAddress) ?? string.Empty;
            }
            else
            {
                lines[1] = GetBuilding(rawAddress) ?? GetStreet(rawAddress) ?? GetLocality(rawAddress) ?? string.Empty;
                lines[2] = GetStreet(rawAddress) ?? GetLocality(rawAddress) ?? string.Empty;
            }
            lines[3] = GetPostTown(rawAddress);
            lines[4] = GetCounty(rawAddress);
            lines[5] = GetPostcode(rawAddress);
            // BUMP
            for (int i = 0; i < 5; i++)
            {
                if (i + 1 < 3)
                {
                    if (lines[i] == lines[i + 1])
                    {
                        if (i + 2 < 3)
                        {
                            lines[i + 1] = lines[i + 2];
                        }

                    }
                }
            }
            // DUMP
            if (lines[2] != string.Empty && lines[2] == lines[1])
            {
                lines[2] = string.Empty;
            }
            if (lines[1] != string.Empty && lines[1] == lines[0])
            {
                lines[1] = string.Empty;
            }
            // MERGE
            if (LooksLikeANumber(lines[0], new[] { "Flat ", "Unit ", "Apartment " }) && lines[1] != string.Empty)
            {
                lines[0] = lines[0] + ((lines[0].Contains("Flat ") || lines[0].Contains("Unit ") || lines[0].Contains("Apartment ")) ? ", " : " ") + lines[1];
                lines[1] = string.Empty;
                if (lines[2] != string.Empty)
                {
                    lines[1] = lines[2];
                    lines[2] = string.Empty;
                }
            }
            if (LooksLikeANumber(lines[1], new[] { "Flat ", "Unit ", "Apartment " }) && lines[2] != string.Empty)
            {
                lines[1] = lines[1] + ((lines[1].Contains("Flat ") || lines[1].Contains("Unit ") || lines[1].Contains("Apartment ")) ? ", " : " ") + lines[2];
                lines[2] = string.Empty;
            }
            // SPREAD
            if (lines[2] == string.Empty)
            {
                if (lines[1] == string.Empty)
                {
                    if (lines[0] != string.Empty && lines[0].Contains(", "))
                    {
                        string[] segments = lines[0].Split(',');
                        lines[1] = segments[segments.Length - 1].Trim();
                        lines[0] = lines[0].Replace(", " + lines[1], string.Empty);
                    }
                }
                else
                {
                    if (lines[1].Contains(", "))
                    {
                        string[] segments = lines[1].Split(',');
                        lines[2] = segments[segments.Length - 1].Trim();
                        lines[1] = lines[1].Replace(", " + lines[2], string.Empty);
                    }
                    else if (lines[0].Contains(", "))
                    {
                        lines[2] = lines[1];
                        string[] segments = lines[0].Split(',');
                        lines[1] = segments[segments.Length - 1].Trim();
                        lines[0] = lines[0].Replace(", " + lines[1], string.Empty);
                    }
                }
            }
            return lines;
        }


        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
    }
}
