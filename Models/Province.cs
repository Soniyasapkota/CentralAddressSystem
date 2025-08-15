using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentralAddressSystem.Models
{
    public class Province
    {
        [Key]
        public Guid ProvinceID { get; set; }

        [Required(ErrorMessage = "Province code is required")]
        [StringLength(50, ErrorMessage = "Province code cannot exceed 50 characters")]
        public string ProvinceCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Province name is required")]
        [StringLength(100, ErrorMessage = "Province name cannot exceed 100 characters")]
        public string ProvinceName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Number of districts is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of districts must be at least 1")]
        public int Noofdistricts { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public Guid CountryID { get; set; }

        [ForeignKey("CountryID")]
        public Country? Country { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}