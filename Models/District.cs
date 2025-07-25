using Microsoft.AspNetCore.Identity;

namespace CentralAddressSystem.Models
{
    public class District
    {
        public int DistrictID { get; set; }
        public string DistrictName { get; set; } = null!;
        public int ProvinceID { get; set; }
        public Province Province { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
