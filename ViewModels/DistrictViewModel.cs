using System;
 using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CentralAddressSystem.ViewModels 
{
    public class DistrictViewModel
    {
        public Guid DistrictID { get; set; }

        [Required(ErrorMessage = "The District Name field is required.")]
        [StringLength(100, ErrorMessage = "District name cannot exceed 100 characters")]
        public string DistrictName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Province field is required.")]
        public Guid ProvinceID { get; set; }

        public string? ProvinceName { get; set; }

        public string? CountryName { get; set; }

        public DateTime CreatedAt { get; set; }

        public SelectList? Provinces { get; set; }
    }

}