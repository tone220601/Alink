using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Portal.Extentions
{
    public class IsValidPostCode: ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            try
            {
                return (
                    Regex.IsMatch((string)value, "(^[A-PR-UWYZa-pr-uwyz][0-9][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)") ||
                    Regex.IsMatch((string)value, "(^[A-PR-UWYZa-pr-uwyz][0-9][0-9][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)") ||
                    Regex.IsMatch((string)value, "(^[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)") ||
                    Regex.IsMatch((string)value, "(^[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][0-9][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)") ||
                    Regex.IsMatch((string)value, "(^[A-PR-UWYZa-pr-uwyz][0-9][A-HJKS-UWa-hjks-uw][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)") ||
                    Regex.IsMatch((string)value, "(^[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][A-Za-z][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)") ||
                    Regex.IsMatch((string)value, "(^[Gg][Ii][Rr][]*0[Aa][Aa]$)")
                    );
            }
            catch
            {
                return false;
            }
        }
    }
}