using Microsoft.AspNetCore.Identity;

namespace CentralAddressSystem.Models
{
    public class LocalBody
    {
        public int LocalBodyID { get; set; }
        public string LocalBodyName { get; set; } = null!;
        public int DistrictID { get; set; }
        public District District { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}