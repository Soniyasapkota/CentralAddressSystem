using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CentralAddressSystem.ViewModels
{
    public class LocalBodyViewModel
    {
        public Guid LocalBodyID { get; set; }

        [Required(ErrorMessage = "The Local Body Name field is required.")]
        [StringLength(100, ErrorMessage = "Local Body name cannot exceed 100 characters")]
        public string LocalBodyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The District field is required.")]
        public Guid DistrictID { get; set; }

        public string? DistrictName { get; set; }

        public string? ProvinceName { get; set; }

        public SelectList? Districts { get; set; }
    }
}