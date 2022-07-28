using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Extentions;

namespace Portal.Models
{
    public class Customer
    {
        public Customer()
        {
            Employer = new List<Employer>();
            RouteOfTravels = new List<RouteOfTravel>();
        }

        [Key]
        public int CustomerId { get; set; }
        //[Required(ErrorMessage = "You must select a Title!")]
        //[RegularExpression("^((?!select).)*$", ErrorMessage = "You must select a Title!")]
        [MaxLength(20)]
        public string Title { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        [MinLength(1, ErrorMessage = "A minimum of 1 character must be supplied")]
        //[DataType(DataType.Text, ErrorMessage = "You may only enter alphabetic letters in this field.")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "You may only enter alphabetic letters in this field.")]
        public string FirstName { get; set; }

        //[Required(ErrorMessage = "Surname is required.")]
        [MinLength(1, ErrorMessage = "A minimum of 1 character must be supplied")]
        //[DataType(DataType.Text, ErrorMessage = "You may only enter alphabetic letters in this field.")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "You may only enter alphabetic letters in this field.")]
        public string Surname { get; set; }

        //[Required(ErrorMessage = "Address line 1 is required.")]
        [MinLength(1), MaxLength(100, ErrorMessage = "You may only enter a maximum of 100 characters.")]
        public string Address1 { get; set; }

        //[Required(ErrorMessage = "A valid address is required.")]
        [MinLength(1)]
        public string Address2 { get; set; }
        public string Address3 { get; set; }

        //[Required(ErrorMessage = "You must enter a valid Town or city.")]
        [MinLength(1)]
        public string Town { get; set; }

        //[Required(ErrorMessage = "You must enter a valid County.")]
        [MinLength(1)]
        public string County { get; set; }

        //[Required(ErrorMessage = "You must enter a valid Postcode.")]
        [MinLength(1)]
        //[RegularExpression("(^gir\\s?0aa$)|(^[A-Z-a-z-[qvx]](\\d{1,2}|[a-hk-y]\\d{1,2}|\\d[a-hjks-uw]|[a-hk-y]\\d[abehmnprv-y])\\s?\\d[a-z-[cikmov]]{2}$)",ErrorMessage = "This is not a valid postcode")]
        [IsValidPostCode]
        public string PostCode { get; set; }
        //[Required(ErrorMessage = "You must enter a valid email address")]
        [DataType(DataType.EmailAddress)]

        //[IsValidEmailAddress(ErrorMessage = "This is not a valid email address")]
        public string Email { get; set; }
        //[Required]
        [System.ComponentModel.DataAnnotations.Compare("Email", ErrorMessage = "Your email address does not match")]
        public string ConfirmEmail { get; set; }

        [IsValidPhoneNumber(ErrorMessage = "This is not a valid phone number.")]
        public string TelePhoneNumber { get; set; }

        public bool Male { get; set; }
        public bool Female { get; set; }
        public bool GenderNotSpecified { get; set; }

        //[Required]
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]

        [Required, IsValidAge]
        public DateTime DateOfBirth { get; set; }

        public string Titles { get; set; }
        public IEnumerable<SelectListItem> Items
        {
            get
            {
                yield return new SelectListItem { Text = "Please Select", Value = "", Selected = true };
                yield return new SelectListItem { Text = "Mr", Value = "Mr" };
                yield return new SelectListItem { Text = "Mrs", Value = "Mrs" };
                yield return new SelectListItem { Text = "Miss", Value = "Miss" };
                yield return new SelectListItem { Text = "Ms", Value = "Ms" };
            }
        }

        public virtual ICollection<Employer> Employer { get; set; }
        public virtual ICollection<RouteOfTravel> RouteOfTravels { get; set; }
        
    }
}