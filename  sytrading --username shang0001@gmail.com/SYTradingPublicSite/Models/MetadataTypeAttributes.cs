using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SYTradingPublicSite.Models
{
    [MetadataType(typeof(CustomerWithMDT))]
    public partial class Customer
    { }

    public class CustomerWithMDT
    {
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(20, ErrorMessage = "Title cannot be longer than 20 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name")]
        [MaxLength(200, ErrorMessage = "First Name cannot be longer than 200 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [Display(Name = "Last Name")]
        [MaxLength(200, ErrorMessage = "Last Name cannot be longer than 200 characters.")]
        public string LastName { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email address is not valid")]
        public string Email { get; set; }

        [Display(Name = "Phone number")]
        [MaxLength(100, ErrorMessage = "Phone number cannot be longer than 100 characters.")]
        public string Phone { get; set; }

        [Display(Name = "Fax number")]
        [MaxLength(100, ErrorMessage = "Fax number cannot be longer than 100 characters.")]
        public string Fax { get; set; }

        [Display(Name = "State / Province")]
        public string Province { get; set; }

        [Display(Name = "ZIP / Postal Code")]
        public string PostalCode { get; set; }
    }

    [MetadataType(typeof(MessageWithMDT))]
    public partial class Message
    { }

    public class MessageWithMDT
    {
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }
    }
}