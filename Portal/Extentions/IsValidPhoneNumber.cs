using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Portal.Extentions
{
    public class IsValidPhoneNumber : ValidationAttribute
    {

        private static readonly Regex RegexMobile =  new Regex(@"^(\+44\s?7\d{3}|\(?07\d{3}\)?)\s?\d{3}\s?\d{3}$");
        private static readonly Regex RegexLandLine = new Regex(@"^\s*\(?(020[7,8]{1}\)?[ ]?[1-9]{1}[0-9{2}[ ]?[0-9]{4})|(0[1-8]{1}[0-9]{3}\)?[ ]?[1-9]{1}[0-9]{2}[ ]?[0-9]{3})\s*$");

        public override bool IsValid(object value)
        {
            if (string.IsNullOrEmpty((string) value))
            {
                return true;
            }
            if (RegexLandLine.Match((string) value).Length > 0)
            {
                return true;
            }
            if (RegexMobile.Match((string)value).Length > 0 )
            {
                return true;
            }
            return false;
        }
    }
}