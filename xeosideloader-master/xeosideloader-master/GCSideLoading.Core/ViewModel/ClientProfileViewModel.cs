using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GCSideLoading.Core.ViewModel
{
    public class ClientProfileViewModel
    {
        public string Id { get; set; }
        [DisplayName("First Name")]
        [Required(ErrorMessage ="First Name Required")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last Name Required")]
        public string LastName { get; set; }
        public string fullName;
        [DisplayName("Full Name")]
        public string FullName {
            get
            {
                fullName = string.Empty;
                if (!string.IsNullOrEmpty(FirstName))
                {
                    fullName = FirstName;
                }
                if (!string.IsNullOrEmpty(LastName))
                {
                    fullName = fullName + " " + LastName;
                }
                return fullName;
            }
        }

        [DisplayName("Client Name")]
        [Required(ErrorMessage = "Client Name Required")]
        public string ClientName { get; set; }

        [DisplayName("URL Slug")]
        //[Required(ErrorMessage = "URL Slug Required")]
        public string GameUrlSlug { get; set; }
        [DisplayName("Email Address")]
        [Required(ErrorMessage = "Email Address Required")]
        [EmailAddress(ErrorMessage ="Email is not valid")]
        public string EmailAddress { get; set; }
        [DisplayName("Phone Number")]
        [Required(ErrorMessage = "Phone Number Required")]
        public string PhoneNo { get; set; }
        [DisplayName("Country Code")]
        //[Required(ErrorMessage = "Country Code Required")]
        public string PhoneCode { get; set; }
        public string internationalNumber;

        [DisplayName("Phone Number")]
        public string InternationalNumber
        {
            get
            {
                internationalNumber = string.Empty;

                if (!string.IsNullOrEmpty(PhoneCode))
                {
                    internationalNumber = PhoneCode;
                }
                if (!string.IsNullOrEmpty(PhoneNo))
                {
                    internationalNumber = internationalNumber + " " + PhoneNo;
                }
                return internationalNumber;
            }
        }

        [DisplayName("Address")]
        public string Address { get; set; }

        [DisplayName("Country")]
        public string Country { get; set; }

        [DisplayName("ZIP")]
        public string ZipCode { get; set; }

        public string AppUserId { get; set; }

        [DisplayName("Client Logo")]
        public string LogoImage { get; set; }

        public string Password { get; set; }
        public string RePassword { get; set; }
    }
}
