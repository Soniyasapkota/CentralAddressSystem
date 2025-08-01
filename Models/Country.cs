using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentralAddressSystem.Models
{
    public class Country
    {
        [Key]
        public int CountryID { get; set; }

        [Required(ErrorMessage = "Country name is required")]
        [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters")]
        public string CountryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country code is required")]
        [StringLength(10, ErrorMessage = "Country code cannot exceed 10 characters")]
        public string CountryCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}