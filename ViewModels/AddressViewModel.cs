using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CentralAddressSystem.ViewModels
{
    public class AddressViewModel
    {
        public Guid AddressID { get; set; }

        [Required(ErrorMessage = "Street is required")]
        [StringLength(200, ErrorMessage = "Street cannot exceed 200 characters")]
        public string Street { get; set; } = null!;

        [Required(ErrorMessage = "Country is required")]
        public Guid CountryID { get; set; }

        public Guid? ProvinceID { get; set; }
        public Guid? DistrictID { get; set; }
        public Guid? LocalBodyID { get; set; }

        public int UserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Dropdown lists
        public SelectList? Countries { get; set; }
        public SelectList? Provinces { get; set; }
        public SelectList? Districts { get; set; }
        public SelectList? LocalBodies { get; set; }

        // Display properties for related entities
        public string? UserName { get; set; }
        public string? CountryName { get; set; }
        public string? ProvinceName { get; set; }
        public string? DistrictName { get; set; }
        public string? LocalBodyName { get; set; }
    }
}