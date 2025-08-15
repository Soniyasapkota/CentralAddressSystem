using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentralAddressSystem.Models
{
    public class Address
    {
        [Key]
        public Guid AddressID { get; set; } // Changed to Guid

        [Required(ErrorMessage = "User ID is required")]
        public int UserID { get; set; } // Remains int, as User PK is int
        [ForeignKey("UserID")]
        public User? User { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public Guid CountryID { get; set; } // Changed to Guid
        [ForeignKey("CountryID")]
        public Country? Country { get; set; }

        public Guid? ProvinceID { get; set; } // Changed to Guid
        [ForeignKey("ProvinceID")]
        public Province? Province { get; set; }

        public Guid? DistrictID { get; set; } // Changed to Guid
        [ForeignKey("DistrictID")]
        public District? District { get; set; }

        public Guid? LocalBodyID { get; set; } // Already Guid
        [ForeignKey("LocalBodyID")]
        public LocalBody? LocalBody { get; set; }

        [Required(ErrorMessage = "Street is required")]
        [StringLength(200, ErrorMessage = "Street cannot exceed 200 characters")]
        public string Street { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}