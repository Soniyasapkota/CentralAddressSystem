using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CentralAddressSystem.Models
{
    public class District
{
    public int DistrictID { get; set; }

        [Required(ErrorMessage = "The District Name field is required.")]
        public string DistrictName { get; set; } = string.Empty;

    [Required(ErrorMessage = "The Province field is required.")]
    public int ProvinceID { get; set; }

    public DateTime CreatedAt { get; set; }

        public Province? Province { get; set; }
}
}
