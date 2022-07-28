using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToolsWebService.CalScan
{
    public class CapScanInterface
    {
        #region Fields

        private const string AmbigString = "AMBIGLIST(LD=CL,LF=S)";
        private string _county = "County";
        private readonly bool _useLocal;
        private readonly string _hostName;
        private string _poolName;
        private readonly string _onDemandServerName;
        private readonly string _onDemandAccountId;
        private readonly string _onDemandClientCode;
        private int _connectHandle;
        private List<string> _availableFields;
        private List<string> _addressElements;
        // Data is returned here
        private Dictionary<string, string> _resultDictionary;
        private Dictionary<string, string> _ambiguityDictionary;
        // Error message returned here
        private string _returnError;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Connection is valid
        /// </summary>
        public bool ConnectionValid
        {
            get { return _connectHandle != 0; }
        }

        /// <summary>
        /// Contains the ambiguity list (uniquepostcode,text)
        /// </summary>
        public Dictionary<string, string> AmbiguityDictionary
        {
            get { return _ambiguityDictionary; }
        }

        /// <summary>
        /// Contains the error text description
        /// </summary>
        public string GetError()
        {
            return _returnError;
        }

        /// <summary>
        /// Return if the poolmanager is being used
        /// </summary>
        public bool IsLocal()
        {
            return _useLocal;
        }

        /// <summary>
        /// returns the field
        /// </summary>
        private string GetDictionaryField(string field)
        {
            if (_resultDictionary.Count == 0)
            {
                _returnError = "return list is empty";
                return string.Empty;
            }

            if (_addressElements.BinarySearch(field) == -1)
            {
                _returnError = field + " not in address elements";
                return string.Empty;
            }
            try
            {
                string result = _resultDictionary[field].Trim();
                return result.ToUpper();
            }
            catch
            {
                _returnError = "An error occurred accessing this element " + field;
                return string.Empty;
            }
        }

        /// <summary>
        /// Contains the PostCode, but may be thr unique postcode
        /// </summary>
        public string GetPostcode()
        {
            string str = GetDictionaryField("POSTCODE");
            return str.Trim();
        }

        /// <summary>
        /// Contains the houseNumber
        /// </summary>
        public string GetHouseNumber()
        {
            char[] trimCharacters = { ';', ' ' };

            var houseNumber = GetDictionaryField("BUILDINGNUMBER");

            if (string.IsNullOrEmpty(houseNumber.Trim(trimCharacters)))
                houseNumber = GetBuildingFormat();

            if (!string.IsNullOrEmpty(GetDictionaryField("ORGANISATION")))
                houseNumber = String.Format("{0} {1}", GetDictionaryField("ORGANISATION"), houseNumber);

            return houseNumber.Trim(trimCharacters);
        }

        /// <summary>
        /// Contains the Building Name
        /// </summary>
        public string GetBuilding()
        {
            string buildingName = GetBuildingFormat();
            // If building was used in house number, don't use it again
            return buildingName == GetHouseNumber() ? string.Empty : buildingName;
        }

        private string GetBuildingFormat()
        {
            return GetDictionaryField("SUBBUILDING").Trim() + (!String.IsNullOrEmpty(GetDictionaryField("SUBBUILDING").Trim()) ? ", " : "") + GetDictionaryField("BUILDINGNAME").Trim();
        }

        /// <summary>
        /// Contains the Street
        /// </summary>
        public string GetStreet()
        {
            string street = GetStreetFormat();
            // If street was used in house number, don't use it again
            return street == GetHouseNumber() ? string.Empty : street;
        }

        private string GetStreetFormat()
        {
            return
                string.Format("{0} {1}", GetDictionaryField("DEPSTREET"), GetDictionaryField("STREET")).Replace("  ",
                                                                                                                " ").
                    Trim();
        }

        /// <summary>
        /// Contains the District
        /// </summary>
        public string GetDistrict()
        {
            string str = GetDictionaryField("DEPLOCALITY") + " " + GetDictionaryField("LOCALITY");
            return str.Trim();
        }

        /// <summary>
        /// Contains the Town
        /// </summary>
        public string GetTown()
        {
            string str = GetDictionaryField("POSTTOWN");
            return str.Trim();
        }

        /// <summary>
        /// Contains the County
        /// </summary>
        public string GetCounty()
        {
            // either ADMINCOUNTY or COUNTY
            string county = GetDictionaryField(_county);
            string str = (county == GetDictionaryField("POSTTOWN") ? string.Empty : county);
            return str.Trim();
        }

        /// <summary>
        /// Contains the County
        /// </summary>
        public string GetAdminCounty()
        {
            // either ADMINCOUNTY or COUNTY
            string str = GetDictionaryField("ADMINCOUNTY");
            return str.Trim();
        }

        public string GetField(string field)
        {
            string str = GetDictionaryField(field);
            return str.Trim();
        }

        /// <summary>
        /// Contains the Ward District
        /// </summary>
        public string GetWardDistrict()
        {
            string str = GetDictionaryField("WDISTRICT");
            return str.Trim();
        }

        #endregion Properties

        #region Constructors

        public CapScanInterface()
        {

        }

        /// <summary>
        /// Initialises the PoolManager and sets connection
        /// </summary>
        public CapScanInterface(string hostName, string poolName)
        {
            _useLocal = true;
            _hostName = hostName;
            _poolName = poolName;
            Initialise();
        }

        /// <summary>
        /// Initialises the OnDemand and sets connection
        /// </summary>
        public CapScanInterface(string onDemandServerName, string onDemandAccountId, string onDemandClientCode)
        {
            _useLocal = false;
            _onDemandClientCode = onDemandClientCode;
            _onDemandServerName = onDemandServerName;
            _onDemandAccountId = onDemandAccountId;
            _hostName = onDemandAccountId;
            Initialise();
        }

        /// <summary>
        /// Sets the optional county lookup field
        /// </summary>
        public void SetCounty(string county)
        {
            _county = county;
        }

        #endregion Constructors

        #region Initialise


        /// <summary>
        /// Call the (now) single routine to Initialise either poolmanger or OnDemand
        /// </summary>
        private void Initialise()
        {
            _connectHandle = InitialiseConnection();
        }

        #endregion Initialise

        #region Methods

        /// <summary>
        /// Calls the main search routine
        /// <param name="postCode"> </param>
        /// <returns> True for success
        /// </returns>
        /// </summary>
        public bool SearchForAddress(string postCode)
        {
            string localAuthority;
            return SearchForAddress(postCode, out localAuthority);
        }

        /// <summary>
        /// Calls the main search routine
        /// <param name="postCode"> </param>
        /// <returns> True for success
        /// </returns>
        /// </summary>
        public bool SearchForAddress(string postCode, out string localAuthority)
        {
            const int timeout = 30;
            string messageText;
            string[] outData;
            _returnError = string.Empty;
            localAuthority = string.Empty;

            // get the connection handle
            // check that we have a valid connection
            if (_connectHandle == 0)
            {
                _returnError = "Connection could not be established";
                return false;
            }
            // clear the error box
            _returnError = "";

            #region Add Formats

            // replace thw ambiguity feild list
            int ambigfieldIndex = ReplaceField("AMBIGLIST", AmbigString);

            #endregion Add Formats

            #region Do Search

            try
            {
                var whereList = new string[1];
                var searchAddress = new string[1];

                // set the search field
                const int searchType = (Int32)Capscan.ClientApi.SearchType.PostcodeLookup;
                whereList[0] = "POSTCODE";

                // Split the search string in postcode and optional ambiguity filter
                var addressSearchSplitArray = postCode.Split(',');

                // Get the input address
                searchAddress[0] = addressSearchSplitArray[0];

                // search for the given address
                outData = Capscan.ClientApi.Search(_connectHandle, searchType, _availableFields.ToArray(), whereList,
                                                   searchAddress, timeout);
            }
            catch (Exception ex)
            {
                messageText = "An exception occurred.\n\n" + ex.Message;

                // show the error message
                _returnError = messageText;
                return false;
            }

            if (outData == null || outData.Length == 0)
            {
                _returnError = "No data found.";

                // disconnect from the poolman
                DisconnectFromPoolman(true);

                return false;
            }

            #endregion Do Search

            #region Handle Errors

            int fieldIndex = Array.BinarySearch(_availableFields.ToArray(), "RESCODE");
            if (outData[fieldIndex] != "")
            {
                int resCode = Convert.ToInt32(outData[fieldIndex]);
                // if RESCODE is not equal to 1 or 4 then display the error message
                // 1 = Address successfully matched to postcode level
                // 4 =  Address successfully matched but Result covers more than one postcode
                switch (resCode)
                {
                    case (int)Capscan.ClientApi.ResCode.Error:
                        messageText = "Error "
                                      + outData[Array.BinarySearch(_availableFields.ToArray(), "ERRNO")]
                                      + ": " + outData[Array.BinarySearch(_availableFields.ToArray(), "ERRTEXT")];

                        // disconnect from the poolman
                        DisconnectFromPoolman(true);

                        _returnError = messageText;
                        return false;

                    case (int)Capscan.ClientApi.ResCode.NoHits:
                        // disconnect from the poolman
                        DisconnectFromPoolman(true);
                        _returnError = "No Hits found.";
                        return false;

                    case (int)Capscan.ClientApi.ResCode.Insufficient:

                        // disconnect from the poolman
                        DisconnectFromPoolman(true);
                        _returnError = "Insufficient information to determine postcode.";
                        return false;

                    case (int)Capscan.ClientApi.ResCode.Foreign:
                        _returnError = "Foreign(address).";
                        return false;
                }
            }

            #endregion Handle Errors

            #region Process Ambiguous data

            // check that there are items in the ambiguity list array.  if the LISTCOUNT field is 
            // empty then there are no items
            if (outData[ambigfieldIndex] != "")
            {
                string ambigLine = outData[ambigfieldIndex];
                ambigLine = ambigLine.Replace("\r\n", "\r");
                string[] addresses = ambigLine.Split('\r');
                if (addresses.Length > 0)
                {
                    _ambiguityDictionary = new Dictionary<string, string>();
                    foreach (string address in addresses)
                    {
                        string addr = address;
                        addr = addr.Replace(",,", ",");
                        addr = addr.Replace(",,", ",");
                        addr = addr.Replace(",,", ",");
                        // remove leading comma
                        addr = addr.Trim(',');
                        string[] parts = addr.Split(';');
                        if (parts.Length == 2)
                        {
                            parts[0] = parts[0].Replace(parts[1], "");
                            parts[0] = parts[0].Replace(",,", ",");
                            //The postcode is often embedded in this string
                            int pos = parts[0].ToLower().IndexOf(parts[1].ToLower(), StringComparison.Ordinal);
                            if (pos != -1)
                            {
                                parts[0] = parts[0].Remove(pos, parts[1].Length + 1);
                            }
                            parts[0] = parts[0].Replace(parts[1], "");
                            _ambiguityDictionary.Add(parts[1], parts[0].Trim(','));
                        }
                    }
                }
            }

            #endregion Process Ambiguous data

            #region resultDictionary

            if (outData[0] != "")
            {
                _resultDictionary = new Dictionary<string, string>();
                foreach (string item in _addressElements)
                {
                    fieldIndex = Array.BinarySearch(_availableFields.ToArray(), item);

                    if (fieldIndex >= 0)
                    {
                        if (outData[fieldIndex].Length > 0)
                            _resultDictionary.Add(_availableFields[fieldIndex], outData[fieldIndex].Trim());
                    }
                }
            }

            #endregion resultDictionary


            // disconnect from the poolman
            DisconnectFromPoolman(false);

            return true;
        }

        private int ReplaceField(string fieldName, string replaceWith)
        {
            int index = Array.BinarySearch(_availableFields.ToArray(), fieldName);
            if (index < 0)
            {
                index = Array.BinarySearch(_availableFields.ToArray(), replaceWith);
                if (index >= 0)
                    return index;
            }
            if (index >= 0)
            {
                _availableFields[index] = replaceWith;
                return index;
            }
            return -1;
        }

        #endregion Methods

        #region Connect/Disconnect

        // Code Supplied by capScan and modified 

        /// <summary>
        /// Calls the routines to connect to poolmanager or OnDemand
        /// <returns> connectionHandle
        /// </returns>
        /// </summary>

        private int InitialiseConnection()
        {
            int conn;
            const int timeout = 30;

            try
            {
                int status = Capscan.ClientApi.Startup(null, 1);

                if (status == 0)
                    return 0;

                int connMode;
                string hostName;
                if (_useLocal)
                {
                    // get the selected host name
                    hostName = _hostName;

                    // set the connection type
                    connMode = (Int32)Capscan.ClientApi.ConnectionMode.Connectionless;
                }
                else
                {
                    // get the required host name for OnDemand
                    hostName = _onDemandServerName + ":" +
                               _onDemandAccountId + ":" +
                               _onDemandClientCode;

                    // set the connection type
                    connMode = (Int32)Capscan.ClientApi.ConnectionMode.OnDemand;
                }

                // call SetConnectionMode to change the default connection mode
                Capscan.ClientApi.SetConnectionMode(0, connMode);

                // set the name/ipaddress of the server
                Capscan.ClientApi.SetParam(0, "HOST", hostName);

                #region redundant code

                #endregion redundant code

                // Connect to the pool manager
                //
                _poolName = "PAF";
                conn = Capscan.ClientApi.Connect(_poolName, "ESP-ETC");
                if (conn == 0)
                    return 0;


                //
                // Get the available list of fields
                //
                // get pool details
                string[] poolDetails = Capscan.ClientApi.PoolInfo(conn, _poolName, timeout);


                string tempBuf = string.Empty;
                // check that we have some pools
                if (poolDetails.Length > 0)
                {
                    // pool info returns all fields as pair values starting with the
                    // field name then it's value, IE: Pool1,RAF,Pool2,PAF....
                    for (int itemCount = 0; itemCount <= poolDetails.Length - 1; itemCount++)
                    {
                        // not all countries have the same address elements (e.g: Australia differs from USA),
                        // so get the list of the address elements for the selected PAF from POOLINFO.
                        if ((String.CompareOrdinal("ELEMENTS", poolDetails[itemCount]) == 0))
                        {
                            tempBuf = poolDetails[(itemCount + 1)];
                            break;
                        }

                        itemCount++;
                    }

                    _addressElements = new List<string> { "ADMINCOUNTY" };

                    if (tempBuf.Length > 0)
                        _addressElements.AddRange(tempBuf.Split(','));

                    _addressElements.Add("WDISTRICT");

                    // add the address elements to the listview
                }

                // get the list o davailable fields
                _availableFields = new List<string>();
                _availableFields.AddRange(Capscan.ClientApi.Columns(conn, timeout));
                _availableFields.Add("FIELDSTATUS");
                // sort array
                _availableFields.Sort();

                if (_availableFields.Count == 0)
                {
                    _returnError = "No available fields";
                    return 0;
                }
            }
            catch
            {
                return 0;
            }

            return conn;
        }

        /// <summary>
        /// Closes the current connection
        /// </summary>

        public void DisconnectFromPoolman(bool force)
        {
            if (!ConnectionValid)
                return;

            if (_useLocal || force)
            {
                // disconnect and cleanup from the connection
                Capscan.ClientApi.Disconnect(_connectHandle);
                Capscan.ClientApi.Cleanup();
                _connectHandle = 0;
            }
        }

        #endregion Connect/Disconnect
    }
}