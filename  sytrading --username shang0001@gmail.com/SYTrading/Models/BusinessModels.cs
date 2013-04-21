using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace SYTrading.Models
{
    public class BusinessDbContext : DbContext
    {
        public DbSet<Glove> Gloves { get; set; }
        public DbSet<ImagePath> ImagePaths { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<GloveApplication> GloveApplications { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Config> Configs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class Glove
    {
        public int GloveID { get; set; }

        [Required(ErrorMessage = "Item No. is required.")]
        [Display(Name = "Item No.")]
        [MaxLength(50, ErrorMessage = "Item No. cannot be longer than 50 characters.")]
        public string ItemNo { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "Sizes is required.")]
        [MaxLength(50, ErrorMessage = "Sizes cannot be longer than 50 characters.")]
        public string Sizes { get; set; }

        [Required(ErrorMessage = "Colors is required.")]
        [MaxLength(200, ErrorMessage = "Colors cannot be longer than 200 characters.")]
        public string Colors { get; set; }

        public bool Released { get; set; }

        [Required(ErrorMessage = "Material is required.")]
        public int MaterialID { get; set; }
        public virtual Material Material { get; set; }
        public virtual ICollection<ImagePath> Images { get; set; }
        public virtual ICollection<GloveApplication> GloveApplications { get; set; }
    }

    public class ImagePath
    {
        public int ImagePathID { get; set; }
        public string Path { get; set; }
        public string HashString { get; set; }
        public string ThumbnailString { get; set; }
        public int GloveID { get; set; }
        public virtual Glove Glove { get; set; }
    }

    public class Material
    {
        public int MaterialID { get; set; }

        [Required(ErrorMessage = "Material Name is required.")]
        [Display(Name = "Material")]
        [MaxLength(100, ErrorMessage = "Material Name cannot be longer than 100 characters.")]
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Application
    {
        public int ApplicationID { get; set; }

        [Required(ErrorMessage = "Application Name is required.")]
        [Display(Name = "Application Name")]
        [MaxLength(100, ErrorMessage = "Application Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [MaxLength(50, ErrorMessage = "Category cannot be longer than 50 characters.")]
        public string Category { get; set; }
        public virtual ICollection<GloveApplication> GloveApplications { get; set; }
    }

    public class GloveApplication
    {
        public int GloveApplicationID { get; set; }
        public int GloveID { get; set; }
        public int ApplicationID { get; set; }
        public virtual Glove Glove { get; set; }
        public virtual Application Application { get; set; }
    }

    public class Customer
    {
        public int CustomerID { get; set; }

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

        public string Address { get; set; }
        public string City { get; set; }

        [Display(Name = "State / Province")]
        public string Province { get; set; }

        [Display(Name = "ZIP / Postal Code")]
        public string PostalCode { get; set; }

        public string Country { get; set; }

        public bool NewsLetter { get; set; }
    }

    public class Message
    {
        public int MessageID { get; set; }
        public int CustomerID { get; set; }
        public string Subject { get; set; }

        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        public virtual Customer Customer { get; set; }
    }

    public partial class Config
    {
        public int ConfigID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Enabled { get; set; }
        public string Level { get; set; }
    }
}