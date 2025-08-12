using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentralAddressSystem.Models
{
    public class Address
    {
        [Key]
        public int AddressID { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public User? User { get; set; } // Nullable to avoid validation issues

        [Required(ErrorMessage = "Country is required")]
        public int CountryID { get; set; }
        [ForeignKey("CountryID")]
        public Country? Country { get; set; } // Nullable to avoid validation issues

        public int? ProvinceID { get; set; }
        [ForeignKey("ProvinceID")]
        public Province? Province { get; set; }

        public int? DistrictID { get; set; }
        [ForeignKey("DistrictID")]
        public District? District { get; set; }

        public Guid? LocalBodyID { get; set; }
        [ForeignKey("LocalBodyID")]
        public LocalBody? LocalBody { get; set; }

        public int? StateID { get; set; }
        [ForeignKey("StateID")]
        public State? State { get; set; }

        public int? ZipID { get; set; }
        [ForeignKey("ZipID")]
        public ZipCode? ZipCode { get; set; }

        [Required(ErrorMessage = "Street is required")]
        [StringLength(200, ErrorMessage = "Street cannot exceed 200 characters")]
        public string Street { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}