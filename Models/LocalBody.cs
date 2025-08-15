using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentralAddressSystem.Models
{
    public class LocalBody
    {
        [Key]
        public Guid LocalBodyID { get; set; }

        [Required(ErrorMessage = "Local Body Name is required")]
        public string LocalBodyName { get; set; } = null!;

        [Required(ErrorMessage = "District is required")]
        public Guid DistrictID { get; set; } // Already Guid

        [ForeignKey("DistrictID")]
        public District? District { get; set; }
    }
}
