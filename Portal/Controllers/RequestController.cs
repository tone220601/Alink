using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Domain.General.Utils;
using Portal.GetRailStations;
using Portal.AddressLookUp;
using Portal.Models;
using Portal.Services;

namespace Portal.Controllers
{
    public class RequestController : Controller
    {
        private AlinkContext db = new AlinkContext();

        //
        // GET: /Request/

        public ActionResult Index()
        {

            return View();
        }
        
        //
        // GET: /Request/Create

        public ActionResult Create()
        {
            return View(new Customer());
        }

        //
        // POST: /Request/Create step 1

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer customer)
        {
            if (customer.TelePhoneNumber == "Example 01472 588999, 01472588999 mobile 07746 952480 07746952480")
            {
                customer.TelePhoneNumber = string.Empty;
            }

           
            if (ModelState.IsValid)
            {
                Session["Customer"] = customer;
                return RedirectToAction("Employer");
            }

            return View(customer);
        }

        // Employer Details Step 2

        [HttpGet]
        public ActionResult Employer()
        {
            return View(new Employer());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Employer(Employer employer)
        {
            if (ModelState.IsValid)
            {
                Session["Employer"] = employer;
                return RedirectToAction("Journey");
            }
            return View(employer);
        }

        // Journey Step 3
        [HttpGet]
        public ActionResult Journey()
        {
            var view = new Journey();

            view.From1 = string.Empty;
            view.To1 = string.Empty;
            view.From2 = string.Empty;
            view.To2 = string.Empty;
            view.From3 = string.Empty;
            view.To3 = string.Empty;
            view.ModesOfTransports = db.ModeOfTravels.OrderByDescending(o => o.ModeOfTransportId).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.ModeOfTransportType,
                    Value = s.ModeOfTransportId.ToString()
                })
                .ToList();

            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Journey(Journey journey)
        {
            var view = new Journey
            {
                From1 = journey.From1,
                To1 = journey.To1,
                From2 = journey.From2,
                To2 = journey.To2,
                From3 = journey.From3,
                To3 = journey.To3,
                ModesOfTransports =
                    db.ModeOfTravels.OrderByDescending(o => o.ModeOfTransportId).ToList()
                        .Select(
                            s =>
                                new SelectListItem
                                {
                                    Text = s.ModeOfTransportType,
                                    Value = s.ModeOfTransportId.ToString()
                                })
                        .ToList(),
            };

            if (journey.ModeOfTransport1 != 4)
            {
                if (journey.ModeOfTransport2 != 4 && journey.From2 == string.Empty || journey.To2 == string.Empty)
                {
                    ModelState.AddModelError("From2",
                        "A From route must be added OR change the Mod of travel back to Please Select");
                    ModelState.AddModelError("To2",
                        "A  To route must be added OR change the Mod of travel back to Please Select");

                }
                if (journey.ModeOfTransport3 != 4 && journey.From3 == string.Empty || journey.To3 == string.Empty)
                {
                    ModelState.AddModelError("From3",
                        "A From route must be added OR change the Mod of travel back to Please Select");
                    ModelState.AddModelError("To3",
                        "A  To route must be added OR change the Mod of travel back to Please Select");
                }
            }

            if (journey.ModeOfTransport1 == 4)
            {
                ModelState.AddModelError("ModeOfTransport1", "A valid type of transport is required.");
            }
            if (ModelState.IsValid)
            {
                Session["Journey"] = journey;
                return RedirectToAction("Confirmation");
            }
            return View(view);
        }

        // Confirm Details Step 4
        [HttpGet]
        public ActionResult Confirmation()
        {
            Confirmation view;
            if (Session["Confirmation"] == null)
            {
                view = GetConfirmation();
            }
            else
            {
                view = (Confirmation)Session["Confirmation"];
            }

            return View(view);
        }

        //
        // GET: /Request/Edit/5

        public ActionResult Edit()
        {
            var view = new EditDetails
            {
                Customer = (Customer)Session["Customer"],
                Employer = (Employer)Session["Employer"],
                Journey = (Journey)Session["Journey"]
            };
            view.Journey.ModesOfTransports = db.ModeOfTravels.OrderByDescending(o => o.ModeOfTransportId).ToList()
                .Select(
                    s =>
                        new SelectListItem
                        {
                            Text = s.ModeOfTransportType,
                            Value = s.ModeOfTransportId.ToString()
                        })
                .ToList();

            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditDetails editDetails)
        {
            if (ModelState.IsValid)
            {
                Session["Customer"] = editDetails.Customer;
                Session["Employer"] = editDetails.Employer;
                Session["Journey"] = editDetails.Journey;
                Session["Confirmed"] = editDetails;
                Session["Confirmation"] = null;
                return RedirectToAction("Confirmation");
            }
            return View(editDetails);
        }

        //
        // POST: /Request/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirmation(Confirmation confirmation)
        {
            if (ModelState.IsValid)
            {
                var processApplicant = new ProccessApplication();
                var customer = (Confirmation)Session["Confirmation"];
                
                //call service get code 
                var view = new EditDetails
                {
                    Customer = (Customer)Session["Customer"],
                    Employer = (Employer)Session["Employer"],
                    Journey = (Journey)Session["Journey"]
                };

                db.Customers.Add(view.Customer);
               

                var confirmed = new Confirmed {RfqId = 1, Reference = processApplicant.CreateRfq(view, customer.Gender)};

                db.SaveChanges();
                Session["Confirmed"] = null;
                Session["Customer"] = null;
                Session["Employer"] = null;
                Session["Journey"] = null;
                Session["Confirmation"] = null;

                return RedirectToAction("Processed", confirmed);
            }
            return View(confirmation);
        }

        public ActionResult Processed(Confirmed confirmed)
        {
            return View(confirmed);
        }

        public Confirmation GetConfirmation()
        {
            var view = new Confirmation();
            var customer = (Customer)Session["Customer"];
            var employer = (Employer)Session["Employer"];
            var journey = (Journey)Session["Journey"];
            
            if (customer != null)
            {
                //customer
                view.Name = customer.Title + " " + customer.FirstName + " " + customer.Surname;
                view.Address1 = customer.Address1;
                view.Address2 = customer.Address2;
                view.Address3 = customer.Address3;
                view.Town =  customer.Town;
                view.County = customer.County;
                view.PostCode = customer.PostCode;
                view.Email = customer.Email;
                view.PhoneNumber = customer.TelePhoneNumber;
                view.Dob = customer.DateOfBirth;
                if (customer.GenderNotSpecified || !customer.GenderNotSpecified && !customer.Male && !customer.Female)
                {
                    view.Gender = 3;
                }
                else if (customer.Male)
                {
                    view.Gender = 1;
                }
                else if (customer.Female)
                {
                    view.Gender = 2;
                }

                //journey
                view.ModeOfTravel1 = db.ModeOfTravels.SingleOrDefault(s => s.ModeOfTransportId == journey.ModeOfTransport1).ModeOfTransportType;
                view.From1 = journey.From1;
                view.To1 = journey.To1;
                view.ModeOfTravel2 = (journey.ModeOfTransport2 == 4) ? "NA" :
                    db.ModeOfTravels.SingleOrDefault(s => s.ModeOfTransportId == journey.ModeOfTransport2).ModeOfTransportType;
                view.From2 = journey.From2;
                view.To2 = journey.To2;
                view.ModeOfTravel3 = (journey.ModeOfTransport3 == 4) ? "NA" :
                    db.ModeOfTravels.SingleOrDefault(s => s.ModeOfTransportId == journey.ModeOfTransport3).ModeOfTransportType;
                view.From3 = journey.From3;
                view.To3 = journey.To3;

                view.ModesOfTransports = db.ModeOfTravels.OrderByDescending(o => o.ModeOfTransportId).ToList()
                .Select(
                    s =>
                        new SelectListItem
                        {
                            Text = s.ModeOfTransportType,
                            Value = s.ModeOfTransportId.ToString()
                        })
                .ToList();

                if (employer != null)
                {
                    view.Employer = employer.EmployerName;
                    view.EmplyerAddress = employer.Address1 + Environment.NewLine + employer.Town + Environment.NewLine + employer.PostCode;
                    if(string.IsNullOrEmpty(employer.EmployeeNumber))
                    {
                        view.EmplyeeNumber = "Not supplied";  
                    }
                else
                    {
                        view.EmplyeeNumber = employer.EmployeeNumber;
                    }
                     
                }
                Session["Confirmation"] = view;
            }
            else
            {
                view.Name = "Not valid Rfq";
            }

            return view;
        }

        protected override void Dispose(bool disposing)
        {
            if (db != null)
            {
                db.Dispose();
                base.Dispose(disposing);
            }

        }

        [HttpPost]
        public JsonResult LookupAddresses(string postcode)
        {
            if (!String.IsNullOrEmpty(postcode))
            {
                using (var client = new AddressLookUpWebService())
                {
                    var response = client.LookupAddresses(postcode, "", 0);
                    if (response != null)
                    {
                        if (response.IsSingleAddress != null && (bool) response.IsSingleAddress)
                        {
                            return Json(response.SingleAddress);  
                        }
                        else if (response.MultipleAddresses.Length > 0)
                        {
                            return Json(DataContractSerialization.Deserialize(response.MultipleAddresses, typeof(Dictionary<string, string>)));
                        }
                        return Json("Empty");
                    }
                }
            }
            return Json(null);
        }
        
        [HttpPost]
        public JsonResult RailStailsJsonResult()
        {
           
                using (var client = new NIRFromToWebService())
                {
                    var response = client.GetRailStations();
                    if (response != null)
                    {

                        var json = new JsonResult();
                       json = Json(/*DataContractSerialization.Deserialize(*/response/*, typeof(Dictionary<string, string>))*/);
                        return json;


                    }
                }
            return Json(null);
        }

        [HttpPost]
        public ActionResult IsValidPostcode(object value)
        {
            try
            {
                var valid = (
                    Regex.IsMatch((string)value, "(^[A-PR-UWYZa-pr-uwyz][0-9][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)") ||
                    Regex.IsMatch((string)value, "(^[A-PR-UWYZa-pr-uwyz][0-9][0-9][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)") ||
                    Regex.IsMatch((string)value, "(^[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)") ||
                    Regex.IsMatch((string)value, "(^[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][0-9][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)") ||
                    Regex.IsMatch((string)value, "(^[A-PR-UWYZa-pr-uwyz][0-9][A-HJKS-UWa-hjks-uw][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)") ||
                    Regex.IsMatch((string)value, "(^[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][A-Za-z][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)") ||
                    Regex.IsMatch((string)value, "(^[Gg][Ii][Rr][]*0[Aa][Aa]$)")
                    );
                return Json(valid);
            }
            catch
            {
                return Json(false);
            }
        }

        [HttpPost]
        public  ActionResult IsValidEmail(string value)
        {
            Regex Regex = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
            if (string.IsNullOrEmpty( value))
            {
                return Json(true);     
            }
            if (!string.IsNullOrEmpty(value))
            {
                var a = Regex.Match( value).Length > 0;
                return Json(a);
            }
            return Json(true);
        }


        [HttpPost]
        public ActionResult CompareEmail(string email, string confirmemail)
        {

            if (string.IsNullOrEmpty(email) )
            {
                return Json(true);
            }

            if (string.IsNullOrEmpty(confirmemail))
            {
                return Json(true);
            }

            if (email != confirmemail)
            {   
                return Json(false);
            }
            return Json(true);
        }

    }
    }

