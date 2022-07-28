using System;
using Domain.Addresses;


namespace ToolsWebService.Model
{
    [Serializable]
    public class AddressLookupResponse
    {
        public AddressLookupResponse()
        {
            Successful = false;
        }

        public AddressLookupResponse(Address address)
        {
            Successful = true;
            IsSingleAddress = true;
            SingleAddress = address;
        }

        public AddressLookupResponse(string addresses)
        {
            Successful = true;
            IsSingleAddress = false;
            MultipleAddresses = addresses;
        }

        public bool Successful { get; set; }
        public bool? IsSingleAddress { get; set; }
        public Address SingleAddress { get; set; }
        public string MultipleAddresses { get; set; }
    }
}