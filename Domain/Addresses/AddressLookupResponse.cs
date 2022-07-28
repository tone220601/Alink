using System;
using System.Collections.Generic;


namespace Domain.Addresses
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

        public AddressLookupResponse(Dictionary<string, string> addresses)
        {
            Successful = true;
            IsSingleAddress = false;
            MultipleAddresses = addresses;
        }

        public bool Successful { get; set; }
        public bool? IsSingleAddress { get; set; }
        public Address SingleAddress { get; set; }
        public Dictionary<string, string> MultipleAddresses { get; set; }
    }
}
