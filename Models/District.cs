using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentralAddressSystem.Models
{
    public class District
    {
        [Key]
        public Guid DistrictID { get; set; }

        [Required(ErrorMessage = "The District Name field is required.")]
        public string DistrictName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Province field is required.")]
        public Guid ProvinceID { get; set; } // Already Guid

        public DateTime CreatedAt { get; set; }

        [ForeignKey("ProvinceID")]
        public Province? Province { get; set; }
    }
}